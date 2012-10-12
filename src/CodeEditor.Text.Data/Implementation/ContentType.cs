using System;
using System.Linq;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Text.Data.Implementation
{
	public class ContentType : IContentType
	{
		private readonly IExportProvider _exportProvider;
		private readonly string _name;
		private readonly IContentTypeDefinition _definition;

		public ContentType(IExportProvider exportProvider, string name, IContentTypeDefinition definition)
		{
			_exportProvider = exportProvider;
			_name = name;
			_definition = definition;
		}

		public string Name
		{
			get { return _name; }
		}

		public IContentTypeDefinition Definition
		{
			get { return _definition; }
		}

		public object GetService(Type contractType)
		{
			return _exportProvider
				.GetExportsWhereMetadata<IContentTypeMetadata>(m => m.Name == _name, contractType)
				.Select(e => e.Value)
				.SingleOrDefault();
		}
	}
}