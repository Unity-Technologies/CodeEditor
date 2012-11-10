using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public static class CodeEditorMenuItems
	{
		[MenuItem("Assets/Code Editor #w")]
		public static void OpenActiveCodeFile()
		{
			CodeEditorWindow.OpenWindowFor("");
		}

		private static string SelectedAssetPath
		{
			get
			{
				if (Selection.activeInstanceID == 0)
					return "";

				var assetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
				return System.IO.Path.GetFullPath(assetPath);
			}
		}
	}
}
