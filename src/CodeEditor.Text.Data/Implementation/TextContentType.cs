namespace CodeEditor.Text.Data.Implementation
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
