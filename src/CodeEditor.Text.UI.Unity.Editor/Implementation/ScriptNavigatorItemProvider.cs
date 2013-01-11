using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(INavigateToItemProvider))]
	internal class ScriptNavigatorItemProvider : INavigateToItemProvider
	{
		private List<INavigateToItem> _allScripts;

		public List<INavigateToItem> Search(string filter)
		{
			InitIfNeeded();

			if(string.IsNullOrEmpty(filter))
				return _allScripts;

			return
				_allScripts.Where(script => script.DisplayText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).
					ToList();
		}

		public void InitIfNeeded()
		{
			if(_allScripts != null)
				return;

			MonoScript[] allscripts = MonoImporter.GetAllRuntimeMonoScripts();

			_allScripts = new List<INavigateToItem>();
			for(int i = 0; i < allscripts.Length; ++i)
			{
				var script = allscripts[i];
				string path = AssetDatabase.GetAssetPath(script.GetInstanceID());
				if(!string.IsNullOrEmpty(path))
				{
					path = System.IO.Path.GetFullPath(path); // get extension
					string fileName = System.IO.Path.GetFileName(path);
					int instanceID = script.GetInstanceID();
					_allScripts.Add(new ScriptNavigatorItem(fileName, instanceID));
				}
			}
		}
	}
}
