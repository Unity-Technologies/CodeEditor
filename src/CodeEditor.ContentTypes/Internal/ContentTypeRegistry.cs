using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.ContentTypes.Internal
{
	[Export(typeof(IContentTypeRegistry))]
	class ContentTypeRegistry : IContentTypeRegistry
	{
		[Import]
		IExportProvider ExportProvider { get; set; }

		[ImportMany]
		Lazy<IContentTypeDefinition, IContentTypeDefinitionMetadata>[] ContentTypeDefinitions { get; set; }

		[ImportMany]
		Lazy<IFileExtensionToContentTypeDefinition, IFileExtensionToContentTypeMetadata>[] FileExtensions { get; set; }

		public IContentType ForFileExtension(string fileExtension)
		{
			var contentTypeName = ContentTypeNameFromFileExtension(fileExtension);
			return contentTypeName == null
				? null
				: ForName(contentTypeName);
		}

		public IContentType ForName(string contentTypeName)
		{
			var definition = ContentTypeDefinitionForName(contentTypeName);
			return definition == null
				? null
				: new ContentType(ExportProvider, contentTypeName, definition);
		}

		private IContentTypeDefinition ContentTypeDefinitionForName(string contentTypeName)
		{
			return ContentTypeDefinitions
				.Where(d => d.Metadata.ContentTypeName == contentTypeName)
				.Select(d => d.Value)
				.SingleOrDefault();
		}

		private string ContentTypeNameFromFileExtension(string fileExtension)
		{
			return FileExtensions
				.Where(e => e.Metadata.FileExtension == fileExtension)
				.Select(e => e.Metadata.ContentTypeName)
				.SingleOrDefault();
		}
	}
}
