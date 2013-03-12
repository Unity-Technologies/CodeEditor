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
	}
}
