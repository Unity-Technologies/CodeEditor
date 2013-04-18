namespace CodeEditor.IO.Internal
{
	public abstract class Resource : IResource
	{
		readonly ResourcePath _path;

		protected Resource(ResourcePath path)
		{
			_path = path.ToAbsolutePath();
		}

		public ResourcePath Path
		{
			get { return _path; }
		}

		public string Location
		{
			get { return _path.Location; }
		}

		public string Extension
		{
			get { return _path.Extension; }
		}

		public abstract bool Exists();
	}
}