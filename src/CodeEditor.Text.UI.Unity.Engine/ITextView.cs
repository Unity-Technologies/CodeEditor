using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextView
	{
		ITextViewDocument Document { get; }
		ITextViewAppearance Appearance { get; }
		ITextViewWhitespace Whitespace { get; }
		ITextViewMargins Margins { get; }
		ISettings Settings { get; }
		IMouseCursorRegions MouseCursorsRegions { get; }
		IMouseCursors MouseCursors  { get ; }
		IFontManager FontManager { get; }
		
		float LineHeight { get; }
		Rect SpanForCurrentCharacter();
		Rect ViewPort { get; set; }
		Vector2 ScrollOffset { get; set; }
		bool HasSelection {get;}
		Position SelectionAnchor {get; set;}
		bool GetSelectionStart (out int row, out int column);
		bool GetSelectionEnd (out int row, out int column);
		bool GetSelectionInDocument (out int pos, out int length);
		Action<int, int, int> Clicked { get; set; }	// row, column, clickcount
		void EnsureCursorIsVisible();
		bool ShowCursor { get; set; }
		void OnGUI();
	}
}
