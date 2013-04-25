using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Composition.Hosting
{
	public class CompositionContainer : ICompositionContainer
	{
		readonly Dictionary<Type, Export[]> _exports = new Dictionary<Type, Export[]>();
		readonly Dictionary<Type, Lazy<object>> _parts = new Dictionary<Type, Lazy<object>>();
		readonly IExportDefinitionProvider _exportDefinitionProvider;
		readonly ImportDefinitionProvider _importDefinitionProvider = new ImportDefinitionProvider();

		public CompositionContainer(Assembly assembly)
			: this(new AssemblyCatalog(assembly))
		{
		}

		public CompositionContainer(params Assembly[] assemblies)
			: this(AssemblyCatalog.For(assemblies))
		{
		}

		public CompositionContainer(IExportDefinitionProvider definitionProvider)
		{
			_exportDefinitionProvider = definitionProvider;
			AddExportedValue<IExportProvider>(this);
		}

		public void AddExportedValue<T>(T value, params object[] metadata)
		{
			AddExport(new Export(new ExportDefinition(typeof(T), value.GetType(), () => metadata ?? EmptyArray.Of<object>()), () => value));
		}

		void AddExport(Export export)
		{
			_exports.Add(export.Definition.ContractType, new[] {export});
		}

		public T GetExportedValue<T>()
		{
			return (T) GetExportedValue(typeof(T));
		}

		public object GetExportedValue(Type contractType)
		{
			var export = GetExport(contractType);
			if (export == null)
				throw NoExportFoundError(contractType);
			return export.Value;
		}

		Export GetExport(Type contractType)
		{
			return GetExports(contractType).SingleOrDefault();
		}

		public IEnumerable<Export> GetExports(Type contractType)
		{
			lock (_exports)
				return DoGetExports(contractType);
		}

		IEnumerable<Export> DoGetExports(Type contractType)
		{
			Export[] existing;
			if (_exports.TryGetValue(contractType, out existing))
				return existing;
			var exports = CreateExportsFor(contractType);
			_exports.Add(contractType, exports);
			return exports;
		}

		Export[] CreateExportsFor(Type contractType)
		{
			return _exportDefinitionProvider
				.GetExports(contractType)
				.Select(e => new Export(e, FactoryFor(e)))
				.ToArray();
		}

		Func<object> FactoryFor(ExportDefinition exportDefinition)
		{
			return AccessorFor(exportDefinition, GetPartFor(exportDefinition.Implementation));
		}

		Lazy<object> GetPartFor(Type implementation)
		{
			lock (_parts)
				return DoGetPartFor(implementation);
		}

		Lazy<object> DoGetPartFor(Type implementation)
		{
			Lazy<object> existing;
			if (_parts.TryGetValue(implementation, out existing))
				return existing;
			var part = NewPartFor(implementation);
			_parts.Add(implementation, part);
			return part;
		}

		Lazy<object> NewPartFor(Type implementation)
		{
			return new Lazy<object>(() => InstantiatePart(implementation));
		}

		object InstantiatePart(Type implementation)
		{
			var importingConstructor = ImportingConstructorOf(implementation);
			var part = importingConstructor != null
				? CreateInstanceThrough(importingConstructor)
				: Activator.CreateInstance(implementation);
			ComposeParts(part);
			return part;
		}

		Func<object> AccessorFor(ExportDefinition exportDefinition, Lazy<object> part)
		{
			return () =>
			{
				try
				{
					return part.Value;
				}
				catch (Exception e)
				{
					throw new CompositionException(e,
						new CompositionError(exportDefinition.ContractType,
							string.Format("Failed to create `{0}' to satisfy `{1}'!",
								exportDefinition.Implementation,
								exportDefinition.ContractType)));
				}
			};
		}

		object CreateInstanceThrough(ConstructorInfo importingConstructor)
		{
			return importingConstructor.Invoke(ExportedValuesFor(importingConstructor).ToArray());
		}

		IEnumerable<object> ExportedValuesFor(ConstructorInfo importingConstructor)
		{
			return importingConstructor.GetParameters().Select(p => GetExportedValue(p.ParameterType));
		}

		static ConstructorInfo ImportingConstructorOf(Type implementation)
		{
			return implementation.InstanceConstructors().SingleOrDefault(IsImportingConstructor);
		}

		static bool IsImportingConstructor(ConstructorInfo c)
		{
			return Attribute.IsDefined(c, typeof(ImportingConstructor));
		}

		void ComposeParts(object part)
		{
			foreach (var import in ImportsOf(part))
				Satisfy(import, part);
		}

		void Satisfy(ImportDefinition importDefinition, object part)
		{
			var exports = GetExportsSatisfying(importDefinition);
			Validate(importDefinition, exports);
			importDefinition.SatisfyWith(exports, part);
		}

		static void Validate(ImportDefinition importDefinition, Export[] exports)
		{
			switch (importDefinition.Cardinality)
			{
				case ImportCardinality.One:
					if (exports.Length == 0)
						throw NoExportFoundError(importDefinition.ContractType);
					if (exports.Length != 1)
						throw TooManyExportsError(importDefinition, exports);
					break;
			}
		}

		static CompositionException TooManyExportsError(ImportDefinition importDefinition, Export[] exports)
		{
			return new CompositionException(
				new CompositionError(importDefinition.ContractType,
					string.Format("Too many exports for `{0}': `{1}'.", importDefinition.ContractType,
						exports.Select(e => e.Definition.Implementation.FullName).ToList())));
		}

		Export[] GetExportsSatisfying(ImportDefinition importDefinition)
		{
			return GetExports(importDefinition.ContractType)
				.Where(importDefinition.IsSatisfiableBy)
				.ToArray();
		}

		IEnumerable<ImportDefinition> ImportsOf(object value)
		{
			return _importDefinitionProvider.ImportsFor(value.GetType());
		}

		static CompositionException NoExportFoundError(Type contractType)
		{
			return new CompositionException(
				new CompositionError(contractType, string.Format("Export `{0}' not found!", contractType)));
		}
	}
}