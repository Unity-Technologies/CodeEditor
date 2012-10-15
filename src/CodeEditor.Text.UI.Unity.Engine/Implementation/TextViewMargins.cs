using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	internal class TextViewMargins : ITextViewMargins
	{
		private readonly ITextViewMargin[] _textViewMargins;

		private const int Spacing = 5;

		public TextViewMargins(IEnumerable<ITextViewMargin> textViewMargins)
		{
			_textViewMargins = textViewMargins.ToArray();
		}

		public void Repaint(ITextViewLine line, Rect lineRect)
		{
			InvokeWithRects(lineRect, (margin, rect) => margin.Repaint(line, rect));
		}

		public void HandleInputEvent(ITextViewLine line, Rect lineRect)
		{
			InvokeWithRects(lineRect, (margin, rect) => margin.HandleInputEvent(line, rect));
		}

		private void InvokeWithRects(Rect lineRect, Action<ITextViewMargin, Rect> invokeMe)
		{
			foreach (var margin in _textViewMargins)
			{
				lineRect.x += Spacing;
				lineRect.width = margin.Width;

				invokeMe(margin, lineRect);
				lineRect.x += margin.Width;
			}
		}

		public float TotalWidth
		{
			get { return _textViewMargins.Sum(m => m.Width) + Spacing * (_textViewMargins.Length + 1); }
		}
	}
}