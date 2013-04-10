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
		public IFile FileFor(string file)
		{
			return new UnityEditorFile(file);
		}

		public IFolder FolderFor(string folder)
		{
			return new Folder(folder, this);
		}

		class UnityEditorFile : File
		{
			public UnityEditorFile(string file)
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
				get { return FullName.Substring(Environment.CurrentDirectory.Length + 1); }
			}
		}
	}
}