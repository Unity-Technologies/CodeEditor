using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IFileSystem))]
	public class FileSystem : IFileSystem
	{
		public IFile FileFor(ResourcePath path)
		{
			return new File(path);
		}

		public IFolder FolderFor(ResourcePath path)
		{
			return new Folder(path, this);
		}
	}
}
