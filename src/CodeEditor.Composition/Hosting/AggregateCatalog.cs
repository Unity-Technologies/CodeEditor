using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Composition.Hosting
{
	public class AggregateCatalog : IExportDefinitionProvider
	{
		private readonly IExportDefinitionProvider[] _exportDefinitionProviders;

		public AggregateCatalog(IEnumerable<IExportDefinitionProvider> exportProviders) : this(exportProviders.ToArray())
		{
		}

		public AggregateCatalog(params IExportDefinitionProvider[] exportDefinitionProviders)
		{
			_exportDefinitionProviders = exportDefinitionProviders;
		}

		public IEnumerable<ExportDefinition> GetExports(Type contractType)
		{
			return _exportDefinitionProviders.SelectMany(provider => provider.GetExports(contractType));
		}
	}
}