using CodeEditor.ContentTypes;

namespace CodeEditor.Languages.UnityScript.ContentType
{
	[ContentTypeDefinition(Name)]
	public class UnityScriptContentType : IContentTypeDefinition
	{
		public const string Name = "UnityScript";
	}

	[FileExtensionToContentType(UnityScriptContentType.Name, ".js")]
	class UnityScriptFileExtension : IFileExtensionToContentTypeDefinition
	{
	}
}