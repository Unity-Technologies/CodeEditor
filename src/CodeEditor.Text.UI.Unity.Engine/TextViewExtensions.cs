using System;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public static class TextViewExtensions
	{
		public static int VisibleLines(this ITextView view)
		{
			return (int) Math.Floor(view.ViewPort.height / view.LineHeight);
		}
	}
}
