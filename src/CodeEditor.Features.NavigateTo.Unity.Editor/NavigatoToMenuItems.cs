using UnityEditor;

namespace CodeEditor.Features.NavigateTo.Unity.Editor
{
	public static class NavigatoToMenuItems
	{
		[MenuItem("Window/NavigateTo %.")]
		public static void OpenNavigateToFileWindow()
		{
			NavigatorWindow.Open();
		}
	}
}
