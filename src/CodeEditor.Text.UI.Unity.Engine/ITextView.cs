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
		void EnsureCursorIsVisible();

		void OnGUI();
	}
}
