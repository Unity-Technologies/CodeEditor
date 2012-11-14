using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public static class CodeEditorMenuItems
	{
		[MenuItem("Window/Code Editor %w")]
		public static void OpenActiveCodeFile()
		{
			CodeEditorWindow.OpenWindowFor("");
		}

		[MenuItem("Window/NavigateTo %e")]
		public static void OpenNavigateToFileWindow()
		{
			NavigateToFileWindow.Open();
		}

		private static string SelectedAssetPath
		{
			get
			{
				if (UnityEditor.Selection.activeInstanceID == 0)
					return "";

				var assetPath = AssetDatabase.GetAssetPath(UnityEditor.Selection.activeInstanceID);
				return System.IO.Path.GetFullPath(assetPath);
			}
		}
	}
}
