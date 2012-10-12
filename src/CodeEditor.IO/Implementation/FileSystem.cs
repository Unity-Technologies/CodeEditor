using CodeEditor.Composition;

namespace CodeEditor.IO.Implementation
{
	[Export(typeof(IFileSystem))]
	class FileSystem : IFileSystem
	{
		public IFile FileFor(string file)
		{
			return new File(file);
		}
	}
}
