using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Reactive;
using CodeEditor.Text.UI;
using UnityEditor;

namespace CodeEditor.Features.NavigateTo.Unity.Editor
{
	[Export(typeof(INavigateToItemProvider))]
	internal class ScriptNavigatorItemProvider : INavigateToItemProvider
	{
		[Import]
		public IFileNavigationService FileNavigationService { get; set; }

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return
				NavigateToItems()
				.ToObservableX()
				.Where(script => script.DisplayText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0);
		}

		public IEnumerable<INavigateToItem> NavigateToItems()
		{
			return (from script in AllMonoScripts()
							let path = AssetPathFor(script)
							where !string.IsNullOrEmpty(path)
							select ScriptNavigatorItemFor(script, path)).Cast<INavigateToItem>();
		}

		private static string AssetPathFor(MonoScript script)
		{
			return AssetDatabase.GetAssetPath(script.GetInstanceID());
		}

		private static MonoScript[] AllMonoScripts()
		{
			return MonoImporter.GetAllRuntimeMonoScripts();
		}

		private ScriptNavigatorItem ScriptNavigatorItemFor(MonoScript script, string path)
		{
			var fullPath = System.IO.Path.GetFullPath(path); // get extension
			var fileName = System.IO.Path.GetFileName(fullPath);
			var instanceID = script.GetInstanceID();
			return new ScriptNavigatorItem(fileName, instanceID, FileNavigationService);
		}
	}
}
