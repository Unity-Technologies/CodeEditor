using System.IO;

namespace CodeEditor.IO.Internal
{
	public class File : Resource, IFile
	{
		public File(ResourcePath path) : base(path)
		{
		}

		public string ReadAllText()
		{
			using (var reader = new StreamReader(System.IO.File.Open(Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
				return reader.ReadToEnd();
		}

		public virtual void WriteAllText(string text)
		{
			System.IO.File.WriteAllText(Location, text);
		}

		public void Delete()
		{
			System.IO.File.Delete(Location);
		}

		public override bool Exists()
		{
			return System.IO.File.Exists(Location);
		}
	}
}
