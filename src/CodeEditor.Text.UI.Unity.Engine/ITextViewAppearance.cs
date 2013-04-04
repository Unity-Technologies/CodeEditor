using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewAppearance
	{
		GUIStyle Background { get; set; }
		GUIStyle Text { get; }
		GUIStyle LineNumber { get; }
		Color LineNumberColor { get; set; }
		Color SelectionColor { get; set; }
		Color BackgroundColor { get; set; }

		event EventHandler Changed;
	}
}
