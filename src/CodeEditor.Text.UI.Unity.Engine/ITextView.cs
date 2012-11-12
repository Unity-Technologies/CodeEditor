using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextView
	{
		ITextViewDocument Document { get; }
		ITextViewAppearance Appearance { get; }
		float LineHeight { get; }
		Rect SpanForCurrentCharacter();

		Rect ViewPort { get; set; }
		Vector2 ScrollOffset { get; set; }
		bool HasSelection {get;}
		void SetSelectionAnchor (int row, int column);			
		bool GetSelectionStart (out int row, out int column);		
		bool GetSelectionEnd (out int row, out int column);
		bool GetSelectionInDocument (out int pos, out int length);
		System.Action<int, int> DoubleClicked {get; set;}			// row, column
		void EnsureCursorIsVisible();
		void OnGUI();
	}
}
