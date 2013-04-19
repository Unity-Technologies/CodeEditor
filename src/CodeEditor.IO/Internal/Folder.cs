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

		public IFile FileFor(ResourcePath relativeFilePath)
		{
			return _fileSystem.FileFor(Path / relativeFilePath);
		}

		public IFolder FolderFor(ResourcePath relativeFilePath)
		{
			return _fileSystem.FolderFor(Path / relativeFilePath);
		}

		public IEnumerable<IFile> SearchFiles(string pattern, SearchOption searchOption)
		{
			return Directory.GetFiles(Location, pattern, searchOption).Select(_ => _fileSystem.FileFor(_));
		}

		public IEnumerable<IFolder> Folders
		{
			get { return Directory.GetDirectories(Location).Select(_ => _fileSystem.FolderFor(_)); }
		}

		public override bool Exists()
		{
			return Directory.Exists(Location);
		}
	}
}