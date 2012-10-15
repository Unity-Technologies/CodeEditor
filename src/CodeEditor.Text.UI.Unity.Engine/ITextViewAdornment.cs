using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewAdornment
	{
		void Draw(ITextViewLine line, Rect lineRect);
	}
}
