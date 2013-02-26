using CodeEditor.IO;
using CodeEditor.IO.Implementation;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	class UnityEditorFileSystem : IFileSystem
	{
		public IFile FileFor(string file)
		{
			return new UnityEditorFile(file);
		}

		public IFolder FolderFor(string folder)
		{
			return new Folder(folder, this);
		}
	}
}
