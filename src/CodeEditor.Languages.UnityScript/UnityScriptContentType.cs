using CodeEditor.Text.Data;

namespace CodeEditor.Languages.UnityScript
{
	[ContentTypeDefinition(Name)]
	class UnityScriptContentType : IContentTypeDefinition
	{
		public const string Name = "UnityScript";
	}

	[FileExtensionToContentType(UnityScriptContentType.Name, ".js")]
	class UnityScriptFileExtension : IFileExtensionToContentTypeDefinition
	{
	}
}