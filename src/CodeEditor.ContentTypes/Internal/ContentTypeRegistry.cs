using System.Collections.Generic;
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

		public IEnumerable<IContentType> ContentTypes
		{
			get { return ContentTypeDefinitions.Select(_ => ForName(_.Metadata.ContentTypeName)); }
		}

		public IEnumerable<string> FileExtensionsFor(IContentType contentType)
		{
			return FileExtensions
				.Where(_ => _.Metadata.ContentTypeName == contentType.Name)
				.Select(_ => _.Metadata.FileExtension);
		}

		IContentTypeDefinition ContentTypeDefinitionForName(string contentTypeName)
		{
			return ContentTypeDefinitions
				.Where(d => d.Metadata.ContentTypeName == contentTypeName)
				.Select(d => d.Value)
				.SingleOrDefault();
		}

		string ContentTypeNameFromFileExtension(string fileExtension)
		{
			return FileExtensions
				.Where(e => e.Metadata.FileExtension == fileExtension)
				.Select(e => e.Metadata.ContentTypeName)
				.SingleOrDefault();
		}
	}
}