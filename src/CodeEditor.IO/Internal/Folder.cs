using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeEditor.IO.Internal
{
	public class Folder : Resource, IFolder
	{
		readonly IFileSystem _fileSystem;

		public Folder(ResourcePath folder, IFileSystem fileSystem) : base(folder)
		{
			_fileSystem = fileSystem;
		}

		public IFile GetFile(ResourcePath relativeFilePath)
		{
			return _fileSystem.GetFile(Path.Combine(relativeFilePath));
		}

		public IFolder GetFolder(ResourcePath relativeFilePath)
		{
			return _fileSystem.GetFolder(Path.Combine(relativeFilePath));
		}

		public IEnumerable<IFile> GetFiles(string pattern, SearchOption searchOption)
		{
			return Directory.GetFiles(Location, pattern, searchOption).Select(_ => _fileSystem.GetFile(_));
		}

		public IEnumerable<IFolder> GetFolders()
		{
			return Directory.GetDirectories(Location).Select(_ => _fileSystem.GetFolder(_));
		}

		public override bool Exists()
		{
			return Directory.Exists(Location);
		}
	}
}