using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewMargin
	{
		float Width { get; }
		void Repaint(ITextViewLine line, Rect marginRect);
		void HandleInputEvent(ITextViewLine line, Rect marginRect);
	}
}