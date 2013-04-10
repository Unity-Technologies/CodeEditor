using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IFileSystem))]
	public class FileSystem : IFileSystem
	{
		public IFile FileFor(string file)
		{
			return new File(file);
		}

		public IFolder FolderFor(string folder)
		{
			return new Folder(folder, this);
		}
	}
}
