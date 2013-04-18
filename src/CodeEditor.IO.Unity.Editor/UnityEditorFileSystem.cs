using System;
using CodeEditor.IO.Internal;
using UnityEditor;
using File = CodeEditor.IO.Internal.File;

namespace CodeEditor.IO.Unity.Editor
{
	/// <summary>
	/// A <see cref="IFileSystem"/> implementation that notifies the Unity
	/// AssetDatabase upon changes.
	/// </summary>
	public class UnityEditorFileSystem : IFileSystem
	{
		class UnityEditorFile : File
		{
			public UnityEditorFile(ResourcePath file)
				: base(file)
			{
			}

			public override void WriteAllText(string text)
			{
				base.WriteAllText(text);
				ReImportAsset();
			}

			void ReImportAsset()
			{
				AssetDatabase.ImportAsset(AssetPath);
			}

			string AssetPath
			{
				get { return Location.Substring(Environment.CurrentDirectory.Length + 1); }
			}
		}

		public IFile GetFile(ResourcePath path)
		{
			return new UnityEditorFile(path);
		}

		public IFolder GetFolder(ResourcePath path)
		{
			return new Folder(path, this);
		}
	}
}