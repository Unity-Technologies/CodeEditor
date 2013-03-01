using System.IO;

namespace CodeEditor.IO.Implementation
{
	public class File : IFile
	{
		private readonly string _fullName;

		public File(string fullName)
		{
			_fullName = Path.GetFullPath(fullName);
		}

		public string FullName
		{
			get { return _fullName; }
		}

		public string Extension
		{
			get { return Path.GetExtension(_fullName); }
		}

		public string ReadAllText()
		{
			using (var reader = new StreamReader(System.IO.File.Open(_fullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
				return reader.ReadToEnd();
		}

		public virtual void WriteAllText(string text)
		{
			System.IO.File.WriteAllText(_fullName, text);
		}

		public void Delete()
		{
			System.IO.File.Delete(_fullName);
		}
	}
}
