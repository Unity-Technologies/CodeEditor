using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	class TextView : ITextView
	{
		const int TopMargin = 6;
		readonly ITextViewDocument _document;
		readonly ITextViewAppearance _appearance;
		private readonly ITextViewAdornments _adornments;

		public TextView(ITextViewDocument document, ITextViewAppearance appearance, ITextViewAdornments adornments)
		{
			_appearance = appearance;
			_adornments = adornments;
			_document = document;
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
				//DebugDrawRowRect(CursorRow);
				DoGUIOnElements();
				MoveCaretOnMouseClick();
			
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

		void MoveCaretOnMouseClick()
		{
			if (Event.current.type != EventType.MouseDown || Event.current.button != 0)
				return;

			var cursorPosition = Event.current.mousePosition;
			if (cursorPosition.x < ViewPort.x || cursorPosition.x > ViewPort.xMax)
				return;

			var row = GetRow (cursorPosition.y);
			if (row >= LineCount)
				return;

			var rect = GetLineRect (row);
			rect.x +=  Margins.TotalWidth;
			GUIContent guiContent = new GUIContent (Line(row).Text);
			var column = LineStyle.GetCursorStringIndex(rect, guiContent, cursorPosition);
			_document.Caret.SetPosition(row, column);
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
