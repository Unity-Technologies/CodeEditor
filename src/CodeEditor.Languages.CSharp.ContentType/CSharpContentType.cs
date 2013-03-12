using CodeEditor.ContentTypes;

namespace CodeEditor.Languages.CSharp.ContentType
{
	[ContentTypeDefinition(Name)]
	public class CSharpContentType : IContentTypeDefinition
	{
		public const string Name = "CSharp";
	}

	[FileExtensionToContentType(CSharpContentType.Name, ".cs")]
	class CSharpFileExtension : IFileExtensionToContentTypeDefinition
	{
	}
}