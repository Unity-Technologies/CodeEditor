using CodeEditor.IO;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	class UnityEditorFileSystem : IFileSystem
	{
		public IFile FileFor(string file)
		{
			return new UnityEditorFile(file);
		}
	}
}
