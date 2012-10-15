using CodeEditor.Text.Data;

namespace CodeEditor.Languages.Boo
{
	[ContentTypeDefinition(Name)]
	class BooContentType : IContentTypeDefinition
	{
		public const string Name = "Boo";
	}

	[FileExtensionToContentType(BooContentType.Name, ".boo")]
	class BooFileExtension : IFileExtensionToContentTypeDefinition
	{
	}
}