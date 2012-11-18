using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public static class CodeEditorMenuItems
	{
		[MenuItem("Window/Code Editor %w")]
		public static void OpenOrFocusCodeEditorWindow()
		{
			CodeEditorWindow.OpenOrFocusExistingWindow ();
		}

		[MenuItem("Window/NavigateTo %e")]
		public static void OpenNavigateToFileWindow()
		{
			NavigatorWindow.Open (typeof(ScriptNavigatorItemProvider));
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
