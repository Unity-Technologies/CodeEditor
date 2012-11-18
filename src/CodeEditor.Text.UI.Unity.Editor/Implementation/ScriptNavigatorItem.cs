using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class ScriptNavigatorItem : INavigatorWindowItem
	{
		public ScriptNavigatorItem(string displayText, int instanceID)
		{
			DisplayText = displayText;
			InstanceID = instanceID;
		}
		public string DisplayText { get; set; }
		public int InstanceID { get; set; }

		public void NavigateTo()
		{
			string path = AssetDatabase.GetAssetPath(InstanceID);
			if (!string.IsNullOrEmpty(path))
			{
				string filePath = System.IO.Path.GetFullPath(path);
				CodeEditorWindow.OpenWindowFor(filePath);
			}

		}
	}
}
