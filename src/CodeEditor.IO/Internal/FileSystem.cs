using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IFileSystem))]
	public class FileSystem : IFileSystem
	{
		public IFile GetFile(ResourcePath path)
		{
			return new File(path);
		}

		public IFolder GetFolder(ResourcePath path)
		{
			return new Folder(path, this);
		}
	}
}
