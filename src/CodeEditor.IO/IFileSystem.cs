using System;
using System.Collections.Generic;
using System.IO;

namespace CodeEditor.IO
{
	public interface IFileSystem : IResourceContainer
	{
	}

	public interface IResourceContainer
	{
		IFile GetFile(ResourcePath path);
		IFolder GetFolder(ResourcePath path);
	}

	public interface IResource
	{
		/// <summary>
		/// Resource path.
		/// </summary>
		ResourcePath Path { get; }

		/// <summary>
		/// Resource extension including the ".".
		/// </summary>
		string Extension { get; }

		bool Exists();
	}

	public interface IFile : IResource
	{
		string ReadAllText();
		void WriteAllText(string text);
		void Delete();
	}

	public interface IFolder : IResource, IResourceContainer
	{
		IEnumerable<IFile> GetFiles(string pattern, SearchOption searchOption);
		IEnumerable<IFolder> GetFolders();
	}

	public static class FileExtensions
	{
		public static TextReader OpenText(this IFile file)
		{
			return new StringReader(file.ReadAllText());
		}
	}

	public struct ResourcePath
	{
		public static implicit operator ResourcePath(string location)
		{
			return new ResourcePath(location);
		}

		public static implicit operator string(ResourcePath path)
		{
			return path.Location;
		}

		public static bool operator ==(ResourcePath left, ResourcePath right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ResourcePath left, ResourcePath right)
		{
			return !left.Equals(right);
		}

		readonly string _location;

		public ResourcePath(string location)
		{
			if (string.IsNullOrEmpty(location))
				throw new ArgumentException("location cannot be null or empty");
			_location = location;
		}

		/// <summary>
		/// Resource name including extension.
		/// </summary>
		public string Name
		{
			get { return Path.GetFileName(Location); }
		}

		/// <summary>
		/// Resource extension including the ".".
		/// </summary>
		public string Extension
		{
			get { return Path.GetExtension(Location); }
		}

		/// <summary>
		/// File system location (absolute or relative) including the resource name and extension.
		/// </summary>
		public string Location
		{
			get { return _location; }
		}

		public ResourcePath Combine(ResourcePath relativePath)
		{
			return Path.Combine(Location, relativePath.Location);
		}

		public ResourcePath ChangeExtension(string newExtension)
		{
			return Path.ChangeExtension(Location, newExtension);
		}

		public ResourcePath ToAbsolutePath()
		{
			return Path.GetFullPath(Location);
		}

		public override string ToString()
		{
			return Location;
		}

		public override bool Equals(object other)
		{
			return other is ResourcePath && Equals((ResourcePath)other);
		}

		public bool Equals(ResourcePath other)
		{
			return string.Equals(Location, other.Location);
		}

		public override int GetHashCode()
		{
			return Location.GetHashCode();
		}
	}
}
