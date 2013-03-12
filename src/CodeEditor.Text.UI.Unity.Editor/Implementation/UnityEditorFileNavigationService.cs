using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IFileNavigationService))]
	class UnityEditorFileNavigationService : IFileNavigationService
	{
		public void NavigateTo(string fileName, IAnchor anchor = null)
		{
			CodeEditorWindow.OpenWindowFor(fileName, PositionFrom(anchor));
		}

		static Position? PositionFrom(IAnchor anchor)
		{
			var positionAnchor = anchor as PositionAnchor;
			if (positionAnchor == null)
				return null;
			return positionAnchor.Position;
		}
	}
}
