using System;
using System.Collections.Generic;
using System.Text;
using CodeEditor.Text.Data;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	/*
	 Plain text [PT] (the actual text document state). Used for: Inserting/deleting char at caret position/selection in document
	 RenderText with whitespaced tabs [RT] Used for mapping between logical caret and visual caret pos
	 RenderColoredText [RCT]: Rich text with whitespaced tabs. Used for rendering
	 
	 CaretPosition state is in PT space
	 Selection state is in PT space
	 On rendering selection get rect by mapping caret from PT pos to RT pos and get Rect from style using RT positions
	 On mouse clicked get PT pos by mapping from RT pos to PT pos
	 */

	class TextView : ITextView
	{
		float TopMargin = 6;
		float LeftMargin;
		readonly ITextViewDocument _document;
		readonly ITextViewAppearance _appearance;
		readonly ITextViewAdornments _adornments;
		readonly ITextViewWhitespace _whitespace;
		readonly IFontManager _fontManager;
		readonly Selection _selection;
		readonly IMouseCursors _mouseCursors;
		readonly IMouseCursorRegions _mouseCursorsRegions;
		readonly ISettings _settings;
		public Action<int, int, int> Clicked {get; set;}
		public Action TextViewEvent { get; set; }
		public bool ShowCursor { get; set; }

		public TextView(
			ITextViewDocument document,
			ITextViewAppearance appearance,
			ITextViewAdornments adornments,
			IMouseCursors mouseCursors,
			IMouseCursorRegions mouseCursorRegions,
			ITextViewWhitespace whitespace,
			ISettings settings, 
			IFontManager fontManager)
		{
			_appearance = appearance;
			_adornments = adornments;
			_document = document;
			_mouseCursors = mouseCursors;
			_mouseCursorsRegions = mouseCursorRegions;
			_whitespace = whitespace;
			_settings = settings;
			_fontManager = fontManager;
			_selection = new Selection(document.Caret);
			_document.Caret.Moved += EnsureCursorIsVisible;
		}

		public ITextViewMargins Margins { get; set; }

		public Rect ViewPort { get; set; }

		public ITextViewWhitespace Whitespace 
		{
			get { return _whitespace; } 
		}

		public IFontManager FontManager 
		{
			get { return _fontManager; } 
		}

		public ISettings Settings
		{
			get { return _settings; }
		}

		public Vector2 ScrollOffset { get; set; }

		public ITextViewDocument Document
		{
			get { return _document; }
		}

		public ITextViewAppearance Appearance
		{
			get { return _appearance; }
		}

		public IMouseCursors MouseCursors
		{
			get { return _mouseCursors; }
		}

		public IMouseCursorRegions MouseCursorsRegions
		{
			get { return _mouseCursorsRegions;}
		}

		public float LineHeight
		{
			get { return _appearance.Text.lineHeight; }
		}

		public bool HasSelection { get {return _selection.HasSelection();}}
		public Position SelectionAnchor 
		{
			get {return _selection.Anchor;}
			set {_selection.Anchor = value;}
		}
		
		public bool GetSelectionStart (out int row, out int column)
		{
			row = column = 0;
			if (!HasSelection)
				return false;

			row = _selection.BeginDrawPos.Row;
			column = _selection.BeginDrawPos.Column;
			return true;
		}

		public bool GetSelectionEnd (out int row, out int column)
		{
			row = column = 0;
			if (!HasSelection)
				return false;

			row = _selection.EndDrawPos.Row;
			column = _selection.EndDrawPos.Column;
			return true;
		}

		public bool GetSelectionInDocument (out int pos, out int length)
		{
			pos = length = 0;
			if (!HasSelection)
				return false;

			Position begin = _selection.BeginDrawPos;
			Position end = _selection.EndDrawPos;

			int startPos = _document.Line (begin.Row).Start + begin.Column;
			int endPos = _document.Line (end.Row).Start + end.Column;
			
			pos = startPos; 
			length = endPos - startPos;
			return true;
		}

		public void OnGUI()
		{
			if (Repainting)
				EraseBackground();

			LeftMargin = LineHeight * 0.5f; // Make left margin proportional with font size

			ScrollOffset = GUI.BeginScrollView(ViewPort, ScrollOffset, ContentRect);
			{
				if (ScrollOffset.y < 0) 
					ScrollOffset = new Vector2(ScrollOffset.x, 0f);

				DoGUIOnElements();
				HandleMouseDragSelection ();
				if (TextViewEvent != null)
					TextViewEvent();

			} GUI.EndScrollView();

			HandleMouseCursorImage();
		}

		private bool verticalScrollbarVisible 
		{
			get { return ContentRect.height > ViewPort.height; }
		}

		private bool horizontalScrollbarVisible
		{
			get { return ContentRect.height > ViewPort.height; }
		}

		private void HandleMouseCursorImage()
		{
			Rect textAreaRect = ViewPort;
			textAreaRect.x += Margins.TotalWidth;
			textAreaRect.width -= Margins.TotalWidth;
			const float kScrollbarWidth = 17f;
			if (verticalScrollbarVisible)
				textAreaRect.width -= kScrollbarWidth;
			if (horizontalScrollbarVisible)
				textAreaRect.height -= kScrollbarWidth;
			_mouseCursorsRegions.AddMouseCursorRegion(textAreaRect, _mouseCursors.Text);
		}

		private void EraseBackground()
		{
			GUIUtils.DrawRect(ViewPort, Appearance.BackgroundColor);
			if (BackgroundStyle != null)
				GUI.Label(ViewPort, GUIContent.none, BackgroundStyle);
		}

		void DoGUIOnElements()
		{
			int firstRow, lastRow;
			GetFirstAndLastRowVisible(LineCount, ScrollOffset.y, ViewPort.height, out firstRow, out lastRow);
			
			DrawSelection (firstRow, lastRow);

			for (var row = firstRow; row <= lastRow; ++row)
			{
				var lineRect = GetLineRect (row); 

				var line = Line(row);
				if (Repainting)
					Margins.Repaint(line, lineRect);
				else
					Margins.HandleInputEvent(line, lineRect);

				lineRect.x += CodeOffset;
				if (Repainting)
					DrawLine(lineRect, row, row + 110101010);
			}
		}

		float CodeOffset
		{
			get { return Margins.TotalWidth + LeftMargin; }
		}

		void HandleMouseDragSelection ()
		{
			int controlID = 666666;
			Event evt = Event.current;
			switch (evt.type)
			{
				case EventType.mouseDown:
					// If dragging outside window and releasing mousedown we do not get a MouseUp event so we 
					// can clear the hotcontrol. We therefore check if we already have hotcontrol and allow mouse down action if so.
					bool alreadyHotcontrol = GUIUtility.hotControl == controlID; 
					if ((GUIUtility.hotControl == 0 || alreadyHotcontrol) && evt.button == 0)	
					{
						Position pos = GetCaretPositionUnderMouseCursor(Event.current.mousePosition);
						if (evt.clickCount == 1)
						{
							GUIUtility.hotControl = controlID;	// Grab mouse focus
							_selection.Clear();
							if (pos.Column >= 0)
							{
								_selection.Anchor = pos;
								_document.Caret.SetPosition(pos.Row, pos.Column);
							}
						}
						if (evt.clickCount == 2)
						{
							_selection.Clear ();
						}

						if (Clicked != null)
							Clicked(pos.Row, pos.Column, evt.clickCount);

						evt.Use();
					}
					break;

				case EventType.mouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						var cursorPosition = Event.current.mousePosition;
						cursorPosition.x = Mathf.Clamp (cursorPosition.x, ViewPort.xMin, ViewPort.xMax); // clamp in x so we drag select while having cursor outside view rect
						Position pos = GetCaretPositionUnderMouseCursor (cursorPosition);
						if (pos.Column >= 0)
						{
							if (_selection.Anchor.Row < 0)
								_selection.Anchor = pos; // init if dragging from outside into the text
							_document.Caret.SetPosition(pos.Row, pos.Column);
							evt.Use();
						}
					}
					break;
				case EventType.mouseUp:
					if (GUIUtility.hotControl == controlID && evt.button == 0)
					{
						Position pos = GetCaretPositionUnderMouseCursor(Event.current.mousePosition);
						if (_selection.Anchor == pos)
							_selection.Clear ();
						GUIUtility.hotControl = 0;
						evt.Use();
					}
					break;
			}
		}

		void DrawSelection (int firstRowVisible, int lastRowVisible)
		{
			if (!_selection.HasSelection())
				return;

			Color selectionColor = _appearance.SelectionColor;

			int startRow = _selection.BeginDrawPos.Row;
			int endRow = _selection.EndDrawPos.Row;
			int startCol = _selection.BeginDrawPos.Column;
			int endCol = _selection.EndDrawPos.Column;

			if (endRow < firstRowVisible || startRow > lastRowVisible)
				return; // Selection outside view

			int startColumn = Whitespace.ConvertToGraphicalCaretColumn(startCol, Line(startRow));
			int endColumn = Whitespace.ConvertToGraphicalCaretColumn(endCol, Line(endRow));

			int loopBegin = firstRowVisible > startRow ? firstRowVisible : startRow;
			int loopEnd = lastRowVisible < endRow ? lastRowVisible : endRow;
			if (loopBegin > loopEnd)
			{
				Debug.LogError("Invalid loop data " + loopBegin + " " + loopEnd);
				return;
			}

			for (int row = loopBegin; row<=loopEnd; row++)
			{
				var line = Line(row);
				string renderText = Whitespace.FormatBaseText(line.Text);
				
				Rect rowRect = GetLineRect(row);
				Rect textSpanRect;
				if (row == startRow && row == endRow)
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, startColumn, endColumn - startColumn);
				}
				else if (row == startRow)
				{
					int len = renderText.Length - startColumn;
					if (len > 0)
					{
						textSpanRect = GetTextSpanRect(rowRect, renderText, startColumn, len);
					}
					else
					{
						// We are at end of line so we get the span for the previous character and
						// use xMax from that character as startpos.
						textSpanRect = GetTextSpanRect(rowRect, renderText, Mathf.Max(0, startColumn - 1), 1);
						textSpanRect.x = textSpanRect.xMax;
						textSpanRect.width = 0;
					}
				}
				else if (row == endRow)
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, 0, endColumn);
				}
				else
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, 0, renderText.Length);
				}

				float extraWidth = (row != endRow) ? LineHeight*0.5f : 0f;		// add extra width for all rows except the last to enhance selection
				rowRect.x = textSpanRect.x + CodeOffset;
				rowRect.width = textSpanRect.width + extraWidth; 	
				GUIUtils.DrawRect (rowRect, selectionColor);
			}
		}

		private static bool Repainting
		{
			get { return Event.current.type == EventType.repaint; }
		}

		void DrawLine(Rect lineRect, int row, int controlID)
		{
			var line = Line(row);

			DrawAdornments(line, lineRect);

			List<int> tabSizes;
			string baseTextFormatted = Whitespace.FormatBaseText(line.Text, out tabSizes);
			string renderText = Whitespace.FormatRichText(line.RichText, tabSizes);
			LineStyle.Draw(lineRect, MissingEngineAPI.GUIContent_Temp(renderText), controlID);

			if (ShowCursor && row == CaretRow)
			{
				int graphicalCaretPos = Whitespace.ConvertToGraphicalCaretColumn(CaretColumn, line, tabSizes);
				LineStyle.DrawCursor(lineRect, MissingEngineAPI.GUIContent_Temp(baseTextFormatted), controlID, graphicalCaretPos);
			}
		}

		private void DrawAdornments(ITextViewLine line, Rect lineRect)
		{
			_adornments.Draw(line, lineRect);
		}

		Rect ContentRect
		{
			get { return new Rect(0, 0, 1000, LineCount * LineHeight + 2 * TopMargin); }
		}

		void GetFirstAndLastRowVisible(int numRows, float topPixel, float heightInPixels, out int firstRowVisible, out int lastRowVisible)
		{
			firstRowVisible = (int)Mathf.Floor(topPixel / LineHeight);
			lastRowVisible = Mathf.Min(firstRowVisible + (int)Mathf.Ceil(heightInPixels / LineHeight), numRows - 1);
		}

		public Position GetCaretPositionUnderMouseCursor (Vector2 cursorPosition)
		{
			if (cursorPosition.x < ViewPort.x || cursorPosition.x > ViewPort.xMax)
				return new Position(-1,-1);

			var row = GetRow(cursorPosition.y);

			if (row >= LineCount)
				row = LineCount-1;

			var rect = GetLineRect(row);
			rect.x += CodeOffset;

			List<int> tabSizes;
			string renderText = Whitespace.FormatBaseText(Line(row).Text, out tabSizes);

			GUIContent guiContent = new GUIContent(renderText);
			
			cursorPosition.y = (rect.yMin + rect.yMax)*0.5f; // use center of row to fix issue with incorrect string index between rows
			var renderColumn = LineStyle.GetCursorStringIndex(rect, guiContent, cursorPosition);
			var column = Whitespace.ConvertToLogicalCaretColumn(renderColumn, Line(row), tabSizes);
			return new Position(row, column);
		}

		public void EnsureCursorIsVisible()
		{
			Vector2 scrollOffset = ScrollOffset;

			var topPixelOfRow = LineHeight * CaretRow;
			var scrollBottom = topPixelOfRow - ViewPort.height + 2 * LineHeight + 2 * TopMargin;
			scrollOffset.y = Mathf.Clamp(scrollOffset.y, scrollBottom, topPixelOfRow);
			var lineRect = GetLineRect(CaretRow);
			var cursorRect = GetTextSpanRect(lineRect, CurrentLine.Text, Mathf.Max(CaretColumn - 2, 0), 1);

			var scrollRight = Mathf.Max(cursorRect.x - ViewPort.width + 40f, 0f);
			scrollOffset.x = Mathf.Clamp(scrollOffset.x, scrollRight, 1000);

			const float distToScroll = 20f;

			if (cursorRect.x > ViewPort.width + scrollOffset.x)
				scrollOffset.x = Mathf.Min(cursorRect.x - ViewPort.width + distToScroll, 1000f);

			if (cursorRect.x < scrollOffset.x + distToScroll)
				scrollOffset.x = Mathf.Max(cursorRect.x - distToScroll, 0f);

			ScrollOffset = scrollOffset;
		}

		public Rect GetCurrentCharacterRect()
		{
			return GetSubstringRect(CaretRow, Math.Max(0, CaretColumn - 1), 1);
		}

		// Input data in document text values
		public Rect GetSubstringRect(int row, int column, int length)
		{
			if (column < 0 || row < 0 || length < 0)
			{
				Debug.LogError("GetSubstringRect: Invalid input: (column " + column + ", length " + length + ", row " + row + ")");
				return new Rect();
			}

			var line = Line(row);
			string renderText = Whitespace.FormatBaseText(line.Text);
			if (renderText.Length == 0)	
			{
				length = 0;
				column = 0;
			}

			int startColumn = Whitespace.ConvertToGraphicalCaretColumn(column, line);
			Rect textRect = GetTextSpanRect(GetLineRect(row), renderText, startColumn, length);
			textRect.x += CodeOffset;
			return textRect;
		}

		// Input data in render text values
		Rect GetTextSpanRect(Rect lineRect, string renderText, int startPosition, int length)
		{
			if (startPosition < 0)
				startPosition = 0;
			if (startPosition >= renderText.Length - 1)
				startPosition = renderText.Length - 1;
			if (startPosition + length >= renderText.Length)
				length = Mathf.Max(1, renderText.Length - startPosition);

			var start = LineStyle.GetCursorPixelPosition(lineRect, MissingEngineAPI.GUIContent_Temp(renderText), startPosition);
			var end = LineStyle.GetCursorPixelPosition(lineRect, MissingEngineAPI.GUIContent_Temp(renderText), startPosition + length);

			return new Rect(start.x, start.y, end.x - start.x, LineHeight);
		}

		int GetRow(float yPos)
		{
			return Mathf.Max(0, Mathf.FloorToInt( (yPos - TopMargin) / LineHeight));
		}

		Rect GetLineRect(int row)
		{
			return new Rect(0, row * LineHeight + TopMargin, 1000000, LineHeight);
		}

		private void DebugDrawRowRect(int row)
		{
			if (row >= 0 && Event.current.type == EventType.repaint)
				GUIUtils.DrawRect(GetLineRect(row), new Color(0.8f, 0.1f, 0.1f, 0.5f));
		}

		int CaretColumn
		{
			get { return _document.Caret.Column; }
		}

		int CaretRow
		{
			get { return _document.Caret.Row; }
		}

		ITextViewLine CurrentLine
		{
			get { return _document.CurrentLine; }
		}

		private int LineCount
		{
			get { return _document.LineCount; }
		}

		private ITextViewLine Line(int row)	
		{
			return _document.Line(row);
		}

		GUIStyle BackgroundStyle
		{
			get { return _appearance.Background; }
		}

		GUIStyle LineStyle
		{
			get { return _appearance.Text; }
		}

		GUIStyle LineNumberStyle
		{
			get { return _appearance.LineNumber; }
		}
	}
}
