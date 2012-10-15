using CodeEditor.Text.Data;

namespace CodeEditor.Languages.CSharp
{
	[ContentTypeDefinition(Name)]
	class CSharpContentType : IContentTypeDefinition
	{
		public const string Name = "CSharp";
	}

	[FileExtensionToContentType(CSharpContentType.Name, ".cs")]
	class CSharpFileExtension : IFileExtensionToContentTypeDefinition
	{
	}
}