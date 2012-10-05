using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeEditor.Composition.Primitives
{
	public interface IExportProvider
	{
		IEnumerable<Export> GetExports(Type contractType);
	}

	public static class ExportProviderExtensions
	{
		public static IEnumerable<Export> GetExportsWhereMetadata<T>(this IExportProvider provider, Func<T, bool> predicate, Type contractType)
		{
			return provider.GetExports(contractType).Where(e => e.Metadata.OfType<T>().Any(predicate));
		}
	}

	public class Export : Lazy<object>, IMetadataProvider
	{
		public Export(ExportDefinition definition, Func<object> valueFactory) : base(valueFactory)
		{
			Definition = definition;
		}

		public ExportDefinition Definition
		{
			get; private set;
		}

		public IEnumerable<object> Metadata
		{
			get { return Definition.Metadata; }
		}
	}
}