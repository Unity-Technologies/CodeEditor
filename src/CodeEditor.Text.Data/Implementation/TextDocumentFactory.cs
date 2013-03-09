using System;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;

namespace CodeEditor.Text.Data.Implementation
{
	[Export(typeof(ITextDocumentFactory))]
	public class TextDocumentFactory : ITextDocumentFactory
	{
		[Import]
		public IContentTypeRegistry ContentTypeRegistry { get; set; }

		public ITextDocument ForFile(IFile file)
		{
			return new TextDocument(file, ContentTypeForFileExtension(file.Extension));
		}

		private IContentType ContentTypeForFileExtension(string fileExtension)
		{
			var contentType = ContentTypeRegistry.ForFileExtension(fileExtension);
			if (contentType == null)
				throw new ArgumentException(string.Format("No content type registered for file extension `{0}'.", fileExtension));
			return contentType;
		}
	}
}
