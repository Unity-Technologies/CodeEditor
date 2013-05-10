using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	internal interface ITextViewAdornments
	{
		void Draw(ITextViewLine line, Rect lineRect);
	}
}
