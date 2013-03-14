using System;
using System.Text;
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
		const int TopMargin = 6;
		readonly ITextViewDocument _document;
		readonly ITextViewAppearance _appearance;
		readonly ITextViewAdornments _adornments;
		readonly ITextViewTabs _tabs;
		readonly Selection _selection;
		readonly IMouseCursors _mouseCursors;
		readonly IMouseCursorRegions _mouseCursorsRegions;
		public Action<int, int> DoubleClicked {get; set;}
		public bool ShowCursor { get; set; }
		public bool ShowWhitespace { get; set; }


		public TextView(ITextViewDocument document, ITextViewAppearance appearance, ITextViewAdornments adornments, IMouseCursors mouseCursors, IMouseCursorRegions mouseCursorRegions)
		{
			_appearance = appearance;
			_adornments = adornments;
			_document = document;
			_mouseCursors = mouseCursors;
			_mouseCursorsRegions = mouseCursorRegions;
			_selection = new Selection(document.Caret);
			_document.Caret.Moved += EnsureCursorIsVisible;
			_tabs = new TextViewTabs(); // TODO: pass in
		}

		public ITextViewMargins Margins { get; set; }

		public Rect ViewPort { get; set; }

		public ITextViewTabs Tabs 
		{
			get { return _tabs; } 
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

			ScrollOffset = GUI.BeginScrollView(ViewPort, ScrollOffset, ContentRect);
			{
				if (ScrollOffset.y < 0) 
					ScrollOffset = new Vector2(ScrollOffset.x, 0f);

				DoGUIOnElements();
				HandleMouseDragSelection ();

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
			GUI.Label(ViewPort, GUIContent.none, BackgroudStyle);
		}

		public Rect SpanForCurrentCharacter()
		{
			return GetTextSpanRect(GetLineRect(CursorRow), CurrentLine.Text, Math.Max(0, CursorPos - 1), 1);
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

				lineRect.x += Margins.TotalWidth;
				if (Repainting)
					DrawLine(lineRect, row, row + 110101010);
			}
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
						if (evt.clickCount == 1)
						{
							GUIUtility.hotControl = controlID;	// Grab mouse focus
							Position pos = GetCaretPositionUnderMouseCursor(Event.current.mousePosition);
							_selection.Clear();
							if (pos.Column >= 0)
							{
								_selection.Anchor = pos;
								_document.Caret.SetPosition(pos.Row, pos.Column);
							}
						}
						if (evt.clickCount == 2)
						{
							if (DoubleClicked != null)
							{
								_selection.Clear ();
								Position pos = GetCaretPositionUnderMouseCursor(Event.current.mousePosition);
								DoubleClicked (pos.Row, pos.Column);
							}
						}
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

			startCol = LogicalCaretPosToGraphicalCaretPos(startRow, startCol);
			endCol = LogicalCaretPosToGraphicalCaretPos(endRow, endCol);

			if (endRow < firstRowVisible || startRow > lastRowVisible)
				return; // Selection outside view

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
				string renderText = Tabs.ReplaceTabsWithWhiteSpaces(line.Text, false);
				
				Rect rowRect = GetLineRect(row);

				Rect textSpanRect;
				if (row == startRow && row == endRow)
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, startCol, endCol - startCol);
				}
				else if (row == startRow)
				{
					int len = renderText.Length - startCol;
					if (len > 0)
					{
						textSpanRect = GetTextSpanRect(rowRect, renderText, startCol, len);
					}
					else
					{
						// We are at end of line so we get the span for the previous character and
						// use xMax from that character as startpos.
						textSpanRect = GetTextSpanRect(rowRect, renderText, Mathf.Max(0, startCol - 1), 1);
						textSpanRect.x = textSpanRect.xMax;
						textSpanRect.width = 0;
					}
				}
				else if (row == endRow)
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, 0, endCol);
				}
				else
				{
					textSpanRect = GetTextSpanRect(rowRect, renderText, 0, renderText.Length);
				}

				float extraWidth = (row != endRow) ? 8f : 0f;		// add extra width for all rows except the last to enhance selection
				rowRect.x = textSpanRect.x + Margins.TotalWidth;
				rowRect.width = textSpanRect.width + extraWidth; 	
				GUIUtils.DrawRect (rowRect, selectionColor);
			}
		}

		private static bool Repainting
		{
			get { return Event.current.type == EventType.repaint; }
		}

	
		int GraphicalCaretPosToLogicalCaretPos(int lineRow, int graphicalCaretPos)
		{
			var line = Line(lineRow);
			string text = line.Text;
			int glyphCounter = 0;
			int HalfNumberOfWhitespacesPerTab = Tabs.NumberOfWhitespacesPerTab / 2;
			for (int i=0; i<=text.Length; i++)
			{
				if (glyphCounter >= graphicalCaretPos)
					return i;

				if (text[i] == '\t')
				{
					if (glyphCounter + HalfNumberOfWhitespacesPerTab >= graphicalCaretPos)
						return i; 
					glyphCounter += Tabs.NumberOfWhitespacesPerTab;
				}
				else
				{
					glyphCounter += 1;
				}
			}
			return -1;
		}

		int LogicalCaretPosToGraphicalCaretPos(int lineRow, int logicalCaretPos)
		{
			var line = Line(lineRow);
			int glyphCounter = 0;
			for (int i = 0; i <= line.Text.Length; i++)
			{
				if (i == logicalCaretPos)
					return glyphCounter;
				glyphCounter += (line.Text[i] == '\t') ? Tabs.NumberOfWhitespacesPerTab : 1;
			}
			return -1;
		}

		void DrawLine(Rect lineRect, int row, int controlID)
		{
			var line = Line(row);

			DrawAdornments(line, lineRect);

			string renderText = Tabs.ReplaceTabsWithWhiteSpaces(line.RichText, ShowWhitespace);
			LineStyle.Draw(lineRect, MissingEngineAPI.GUIContent_Temp(renderText), controlID);

			if (ShowCursor && row == CursorRow)
			{
				string cursorText = Tabs.ReplaceTabsWithWhiteSpaces(line.Text, false);
				int graphicalCaretPos = LogicalCaretPosToGraphicalCaretPos(row, CursorPos);
				LineStyle.DrawCursor(lineRect, MissingEngineAPI.GUIContent_Temp(cursorText), controlID, graphicalCaretPos);
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

		Position GetCaretPositionUnderMouseCursor (Vector2 cursorPosition)
		{
			if (cursorPosition.x < ViewPort.x || cursorPosition.x > ViewPort.xMax)
				return new Position(-1,-1);

			var row = GetRow(cursorPosition.y);
			
			if (row >= LineCount)
				row = LineCount-1;

			var rect = GetLineRect(row);
			rect.x += Margins.TotalWidth;

			string renderText = Tabs.ReplaceTabsWithWhiteSpaces (Line(row).Text, false);
			GUIContent guiContent = new GUIContent(renderText);
			
			cursorPosition.y = (rect.yMin + rect.yMax)*0.5f; // use center of row to fix issue with incorrect string index between rows
			var renderColumn = LineStyle.GetCursorStringIndex(rect, guiContent, cursorPosition);
			var column = GraphicalCaretPosToLogicalCaretPos(row, renderColumn);
			return new Position(row, column);
		}

		public void EnsureCursorIsVisible()
		{
			Vector2 scrollOffset = ScrollOffset;

			var topPixelOfRow = LineHeight * CursorRow;
			var scrollBottom = topPixelOfRow - ViewPort.height + 2 * LineHeight + 2 * TopMargin;
			scrollOffset.y = Mathf.Clamp(scrollOffset.y, scrollBottom, topPixelOfRow);
			var lineRect = GetLineRect(CursorRow);
			var cursorRect = GetTextSpanRect(lineRect, CurrentLine.Text, Mathf.Max(CursorPos - 2, 0), 1);

			var scrollRight = Mathf.Max(cursorRect.x - ViewPort.width + 40f, 0f);
			scrollOffset.x = Mathf.Clamp(scrollOffset.x, scrollRight, 1000);

			const float distToScroll = 20f;

			if (cursorRect.x > ViewPort.width + scrollOffset.x)
				scrollOffset.x = Mathf.Min(cursorRect.x - ViewPort.width + distToScroll, 1000f);

			if (cursorRect.x < scrollOffset.x + distToScroll)
				scrollOffset.x = Mathf.Max(cursorRect.x - distToScroll, 0f);

			ScrollOffset = scrollOffset;
		}

		Rect GetTextSpanRect(Rect lineRect, string text, int startPos, int length)
		{
			if (startPos < 0)
				startPos = 0;
			if (startPos >= text.Length - 1)
				startPos = text.Length - 1;
			if (startPos + length >= text.Length)
				length = Mathf.Max(1, text.Length - startPos);

			var start = LineStyle.GetCursorPixelPosition(lineRect, MissingEngineAPI.GUIContent_Temp(text), startPos);
			var end = LineStyle.GetCursorPixelPosition(lineRect, MissingEngineAPI.GUIContent_Temp(text), startPos + length);

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

		int CursorPos
		{
			get { return _document.Caret.Column; }
		}

		int CursorRow
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

		GUIStyle BackgroudStyle
		{
			get { return _appearance.Background;  }
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
