using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewAppearance
	{
		GUIStyle Background { get; }
		GUIStyle Text { get; }
		GUIStyle LineNumber { get; }
		Color LineNumberColor { get; }
	}
}