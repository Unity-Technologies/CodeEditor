using System;
using CodeEditor.IO.Internal;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class UnityEditorFile : File
	{
		public UnityEditorFile(string file) : base(file)
		{
		}

		public override void WriteAllText(string text)
		{
			base.WriteAllText(text);
			ReImportAsset();
		}

		private void ReImportAsset()
		{
			AssetDatabase.ImportAsset(AssetPath);
		}

		private string AssetPath
		{
			get { return FullName.Substring(Environment.CurrentDirectory.Length + 1); }
		}
	}
}