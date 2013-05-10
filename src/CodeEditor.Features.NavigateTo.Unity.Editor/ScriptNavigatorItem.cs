using CodeEditor.Text.UI;
using UnityEditor;

namespace CodeEditor.Features.NavigateTo.Unity.Editor
{
	public class ScriptNavigatorItem : INavigateToItem
	{
		readonly IFileNavigationService _fileNavigationService;

		public ScriptNavigatorItem(string displayText, int instanceID, IFileNavigationService fileNavigationService)
		{
			_fileNavigationService = fileNavigationService;
			DisplayText = displayText;
			InstanceID = instanceID;
		}

		public string DisplayText { get; set; }

		public int InstanceID { get; set; }

		public void NavigateTo()
		{
			var path = AssetDatabase.GetAssetPath(InstanceID);
			if (!string.IsNullOrEmpty(path))
				_fileNavigationService.NavigateTo(System.IO.Path.GetFullPath(path));
		}
	}
}
