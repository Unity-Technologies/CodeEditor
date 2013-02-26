using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeEditor.IO.Implementation
{
	public class Folder : IFolder
	{
		private readonly IFileSystem _fileSystem;
		private readonly string _folder;

		public Folder(string folder, IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
			_folder = folder;
		}

		public IEnumerable<IFile> GetFiles(string pattern, SearchOption searchOption)
		{
			return Directory.GetFiles(_folder, pattern, searchOption).Select(_fileSystem.FileFor);
		}
	}
}