using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public static class CodeEditorMenuItems
	{
		[MenuItem("Assets/Code Editor")]
		public static void OpenActiveCodeFile()
		{
			CodeEditorWindow.OpenWindowFor(SelectedAssetPath);
		}

		private static string SelectedAssetPath
		{
			get
			{
				var assetPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
				return System.IO.Path.GetFullPath(assetPath);
			}
		}
	}
}