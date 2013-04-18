using CodeEditor.ContentTypes;

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