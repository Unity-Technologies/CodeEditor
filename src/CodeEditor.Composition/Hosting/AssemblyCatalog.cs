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

		static IEnumerable<IExportDefinitionProvider> AssemblyCatalogsFor(IEnumerable<Assembly> assemblies)
		{
			return assemblies
				.Where(assembly => assembly.IsSafeForComposition())
				.Select(assembly => (IExportDefinitionProvider) new AssemblyCatalog(assembly));
		}

		Dictionary<Type, ExportDefinition[]> _exports;
		readonly Assembly _assembly;

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

		Dictionary<Type, ExportDefinition[]> Exports()
		{
			lock (this)
				return _exports ?? (_exports = ComputeExports());
		}

		Dictionary<Type, ExportDefinition[]> ComputeExports()
		{
			return _assembly
				.GetTypes()
				.SelectMany<Type, ExportDefinition>(ExportsFromType)
				.Where(e => e != null)
				.GroupBy(e => e.ContractType)
				.ToDictionary(e => e.Key, e => e.ToArray());
		}

		IEnumerable<ExportDefinition> ExportsFromType(Type implementation)
		{
			foreach (var attribute in ExportAttributesFrom(implementation))
				yield return new ExportDefinition(attribute.ContractType ?? implementation, implementation);

			foreach (var baseType in implementation.BaseTypes())
				foreach (var inheritedAttribute in InheritedExportAttributesFrom(baseType))
					yield return new ExportDefinition(inheritedAttribute.ContractType ?? baseType, implementation);
		}

		static IEnumerable<ExportAttribute> ExportAttributesFrom(Type implementation)
		{
			return CustomAttribute<ExportAttribute>.AllFrom(implementation);
		}

		static IEnumerable<InheritedExportAttribute> InheritedExportAttributesFrom(Type baseType)
		{
			return CustomAttribute<InheritedExportAttribute>.AllFrom(baseType);
		}

		static readonly ExportDefinition[] NoExportsDefinition = new ExportDefinition[0];
	}
}