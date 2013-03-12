using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewAppearance
	{
		int[] GetSupportedFontSizes();
		void SetFontSize(int fontSize);
		GUIStyle Background { get; }
		GUIStyle Text { get; }
		GUIStyle LineNumber { get; }
		Color LineNumberColor { get; }
		Color SelectionColor { get; }

		event EventHandler Changed;
	}
}
