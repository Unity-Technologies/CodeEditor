using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Composition.Hosting
{
	public class AssemblyCatalog : IExportDefinitionProvider
	{
		public static IExportDefinitionProvider For(IEnumerable<Assembly> assemblies)
		{
			return new AggregateCatalog(AssemblyCatalogsFor(assemblies));
		}

		private static IEnumerable<IExportDefinitionProvider> AssemblyCatalogsFor(IEnumerable<Assembly> assemblies)
		{
			return assemblies
				.Where(assembly => assembly.IsSafeForComposition())
				.Select(assembly => (IExportDefinitionProvider) new AssemblyCatalog(assembly));
		}

		private Dictionary<Type, ExportDefinition[]> _exports;
		private readonly Assembly _assembly;

		public AssemblyCatalog(Assembly assembly)
		{
			_assembly = assembly;
		}

		public IEnumerable<ExportDefinition> GetExports(Type contractType)
		{
			ExportDefinition[] exportsDefinition;
			return Exports().TryGetValue(contractType, out exportsDefinition)
				? exportsDefinition
				: NoExportsDefinition;
		}

		private Dictionary<Type, ExportDefinition[]> Exports()
		{
			lock (this)
				return _exports ?? (_exports = ComputeExports());
		}

		private Dictionary<Type, ExportDefinition[]> ComputeExports()
		{
			return _assembly
				.GetTypes()
				.SelectMany<Type, ExportDefinition>(ExportsFromType)
				.Where(e => e != null)
				.GroupBy(e => e.ContractType)
				.ToDictionary(e => e.Key, e => e.ToArray());
		}

		private IEnumerable<ExportDefinition> ExportsFromType(Type implementation)
		{
			return CustomAttribute<ExportAttribute>
				.AllFrom(implementation)
				.Select(attribute => new ExportDefinition(attribute.ContractType ?? implementation, implementation));
		}

		private static readonly ExportDefinition[] NoExportsDefinition = new ExportDefinition[0];
	}
}
