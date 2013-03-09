namespace CodeEditor.ContentTypes.Internal
{
	[ContentTypeDefinition("text")]
	class TextContentType : IContentTypeDefinition
	{
	}

	[FileExtensionToContentType("text", ".txt")]
	class TextFileExtension
	{
	}
}
