using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	interface IListItemGUI
	{
		float ItemHeight { get; set; }
		void OnGUI(Rect rect, IListItem item, bool selected);
	}
}
