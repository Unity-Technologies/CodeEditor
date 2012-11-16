using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	class ScriptFilePathProvider : IFilePathProvider
	{
		List<FilePathProviderItem> _allScripts;

		public List<FilePathProviderItem> GetItems (string filter)
		{
			InitIfNeeded ();

			if (string.IsNullOrEmpty(filter))
				return _allScripts;

			return _allScripts.Where(script => script.DisplayText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
		}


		public bool GetFileAndLineNumber (object userData, out string filePath, out int lineNumber)
		{
			int instanceID = (int)userData;
			string path = AssetDatabase.GetAssetPath(instanceID);
			if (!string.IsNullOrEmpty (path))
			{
				filePath = System.IO.Path.GetFullPath(path);
				lineNumber = 0;
				return true;
			}

			filePath = "";
			lineNumber = 0;
			return false;
		}

		public void InitIfNeeded ()
		{
			if (_allScripts != null)
				return;

			MonoScript[] allscripts = MonoImporter.GetAllRuntimeMonoScripts();

			_allScripts = new List<FilePathProviderItem>();
			for (int i = 0; i < allscripts.Length; ++i)
			{
				var script = allscripts[i];
				string path = AssetDatabase.GetAssetPath(script.GetInstanceID());
				if (!string.IsNullOrEmpty(path))
				{
					path = System.IO.Path.GetFullPath(path); // get extension
					string fileName = System.IO.Path.GetFileName(path);
					int userData = script.GetInstanceID();
					_allScripts.Add(new FilePathProviderItem(fileName, userData));
				}

			}
		}
	}
}
