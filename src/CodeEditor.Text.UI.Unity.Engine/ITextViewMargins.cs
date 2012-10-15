using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewMargins
	{
		void Repaint(ITextViewLine line, Rect lineRect);
		float TotalWidth { get; }
		void HandleInputEvent(ITextViewLine line, Rect lineRect);
	}
}