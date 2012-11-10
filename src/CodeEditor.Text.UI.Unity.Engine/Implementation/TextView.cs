using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	struct Vector2i 
	{
		public Vector2i (int X, int Y)
		{
			x = X;
			y = Y;
		}
		public int x,y;

		public override string ToString ()
		{
			return string.Format ("({0},{1})", x,y);
		}
		
	}

	class SelectionData
	{
		public Vector2i FixedPos {get; set;}
		public Vector2i ActivePos {get; set;}

		public Vector2i BeginDrawPos
		{
			get 
			{
				return new Vector2i (
					(FixedPos.y < ActivePos.y || FixedPos.x < ActivePos.x) ? FixedPos.x : ActivePos.x,
					FixedPos.y < ActivePos.y ? FixedPos.y : ActivePos.y);
			}
		}

		public Vector2i EndDrawPos
		{
			get
			{
				return new Vector2i(
					(ActivePos.y > FixedPos.y  || FixedPos.x < ActivePos.x) ? ActivePos.x : FixedPos.x,
					ActivePos.y > FixedPos.y ? ActivePos.y : FixedPos.y);
			}
		}

		public void Clear ()
		{
			FixedPos = ActivePos = new Vector2i(-1,-1);
		}

		public bool HasSelection ()
		{
			return FixedPos.y >= 0 && !(FixedPos.y == ActivePos.y && FixedPos.x == ActivePos.x);
		}

		public override string ToString ()
		{
			return string.Format ("draw {0} -> {1},  fixed: {2}, active {3}", BeginDrawPos, EndDrawPos, FixedPos, ActivePos);
		}

	}
	class TextView : ITextView
	{
		const int TopMargin = 6;
		readonly ITextViewDocument _document;
		readonly ITextViewAppearance _appearance;
		private readonly ITextViewAdornments _adornments;
		readonly SelectionData _selection;

		public TextView(ITextViewDocument document, ITextViewAppearance appearance, ITextViewAdornments adornments)
		{
			_appearance = appearance;
			_adornments = adornments;
			_document = document;
			_selection = new SelectionData();
			_selection.Clear ();
		}

		public ITextViewMargins Margins { get; set; }

		public Rect ViewPort { get; set; }

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
			get { return 18f; }
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
					if (GUIUtility.hotControl == 0 && evt.button == 0)
					{
						GUIUtility.hotControl = controlID;	// Grab mouse focus
						Vector2i pos = GetCaretPositionUnderMouseCursor ();
						_selection.Clear ();
						if (pos.x >= 0)
						{
							_selection.FixedPos = pos;
							_selection.ActivePos = pos;
							_document.Caret.SetPosition(pos.y, pos.x);
						}
						evt.Use();
					}
					break;

				case EventType.mouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						Vector2i pos = GetCaretPositionUnderMouseCursor ();
						if (pos.x >= 0)
						{
							_selection.ActivePos = pos;
							if (_selection.FixedPos.y < 0)
								_selection.FixedPos = pos; // init if dragging from outside into the text
							_document.Caret.SetPosition(pos.y, pos.x);
						}
						GUI.changed = true;
						evt.Use();
					}
					break;
				case EventType.mouseUp:
					if (GUIUtility.hotControl == controlID && evt.button == 0)
					{
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

			Color selectionColor = new Color(80/255f, 80/255f, 80/255f, 1f);

			int startRow = _selection.BeginDrawPos.y;
			int endRow = _selection.EndDrawPos.y;
			int startCol = _selection.BeginDrawPos.x;
			int endCol = _selection.EndDrawPos.x;

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

				Rect rowRect = GetLineRect(row);

				Rect textSpanRect;
				if (row == startRow && row == endRow)
				{
					textSpanRect = GetTextSpanRect(rowRect, line.Text, startCol, endCol - startCol);
				}
				else if (row == startRow)
				{
					textSpanRect  = GetTextSpanRect (rowRect, line.Text, startCol, line.Text.Length - startCol);
				}
				else if (row == endRow)
				{
					textSpanRect = GetTextSpanRect (rowRect, line.Text, 0, endCol);
				}
				else
				{
					textSpanRect = GetTextSpanRect(rowRect, line.Text, 0, line.Text.Length);
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

		void DrawLine(Rect lineRect, int row, int controlID)
		{
			var line = Line(row);

			DrawAdornments(line, lineRect);

			LineStyle.Draw(lineRect, MissingEngineAPI.GUIContent_Temp(line.RichText), controlID);

			if (row == CursorRow)
			{
				LineStyle.DrawCursor(lineRect, MissingEngineAPI.GUIContent_Temp(line.Text), controlID, CursorPos);

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

		Vector2i GetCaretPositionUnderMouseCursor ()
		{
			var cursorPosition = Event.current.mousePosition;
			if (cursorPosition.x < ViewPort.x || cursorPosition.x > ViewPort.xMax)
				return new Vector2i(-1,-1);

			var row = GetRow(cursorPosition.y);
			
			if (row >= LineCount)
				row = LineCount-1;

			var rect = GetLineRect(row);
			rect.x += Margins.TotalWidth;
			GUIContent guiContent = new GUIContent(Line(row).Text);
			cursorPosition.y = (rect.yMin + rect.yMax)*0.5f; // use center of row to fix issue with incorrect string index between rows
			var column = LineStyle.GetCursorStringIndex(rect, guiContent, cursorPosition);
			return new Vector2i(column, row);
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
			return new Rect(0, row * LineHeight  + TopMargin, 1000, LineHeight);
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
