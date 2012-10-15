using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	class TextView : ITextView
	{
		const int TopMargin = 6;
		readonly ITextViewDocument _document;
		readonly ITextViewAppearance _appearance;
		private Vector2 _scrollOffset;
		private readonly ITextViewAdornments _adornments;

		public TextView(ITextViewDocument document, ITextViewAppearance appearance, ITextViewAdornments adornments)
		{
			_appearance = appearance;
			_adornments = adornments;
			_document = document;
		}

		public ITextViewMargins Margins { get; set; }

		public Rect ViewPort { get; set; }

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

			_scrollOffset = GUI.BeginScrollView(ViewPort, _scrollOffset, ContentRect);

			DoGUIOnElements();

			MoveCaretOnMouseClick();

			GUI.EndScrollView();
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
			GetFirstAndLastRowVisible(LineCount, _scrollOffset.y, ViewPort.height, out firstRow, out lastRow);

			for (var row = firstRow; row <= lastRow; ++row)
			{
				var lineRect = new Rect(0, row * LineHeight + TopMargin, 1000, LineHeight);

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
				LineStyle.DrawCursor(lineRect, MissingEngineAPI.GUIContent_Temp(line.Text), controlID, CursorPos);
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

			var row = Mathf.FloorToInt(cursorPosition.y / LineHeight);
			if (row >= LineCount)
				return;

			var rect = GetLineRect(row);
			rect.x += 25f + Margins.TotalWidth;
			var guiContent = MissingEngineAPI.GUIContent_Temp(Line(row).Text);
			var column = LineStyle.GetCursorStringIndex(rect, guiContent, cursorPosition);
			_document.Caret.SetPosition(row, column);
		}

		public void EnsureCursorIsVisible()
		{
			var topPixelOfRow = LineHeight * CursorRow;
			var scrollBottom = topPixelOfRow - ViewPort.height + 2 * LineHeight + 2 * TopMargin;
			_scrollOffset.y = Mathf.Clamp(_scrollOffset.y, scrollBottom, topPixelOfRow);
			var lineRect = GetLineRect(CursorRow);
			var cursorRect = GetTextSpanRect(lineRect, CurrentLine.Text, Mathf.Max(CursorPos - 2, 0), 1);

			var scrollRight = Mathf.Max(cursorRect.x - ViewPort.width + 40f, 0f);
			_scrollOffset.x = Mathf.Clamp(_scrollOffset.x, scrollRight, 1000);

			const float distToScroll = 20f;

			if (cursorRect.x > ViewPort.width + _scrollOffset.x)
				_scrollOffset.x = Mathf.Min(cursorRect.x - ViewPort.width + distToScroll, 1000f);

			if (cursorRect.x < _scrollOffset.x + distToScroll)
				_scrollOffset.x = Mathf.Max(cursorRect.x - distToScroll, 0f);
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

		Rect GetLineRect(int row)
		{
			return new Rect(0, row * LineHeight, 1000, LineHeight);
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
