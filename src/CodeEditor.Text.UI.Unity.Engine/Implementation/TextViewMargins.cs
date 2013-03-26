using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewMargins : ITextViewMargins
	{
		public TextViewMargins(IEnumerable<ITextViewMargin> textViewMargins)
		{
			Margins = textViewMargins.ToArray();
			Spacing = 5;
		}

		public ITextViewMargin[] Margins { get; set; }

		public int Spacing { get; set; }

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
			foreach (var margin in Margins)
			{
				if (!margin.Visible)
					continue;
				lineRect.x += Spacing;
				lineRect.width = margin.Width;

				invokeMe(margin, lineRect);
				lineRect.x += margin.Width;
			}
		}

		public float TotalWidth
		{
			get
			{
				float totalWidth = 0;
				int count = 0;
				foreach (var margin in Margins)
				{
					if (!margin.Visible)
						continue;
					totalWidth += margin.Width;
					count++;
				}
				return count == 0 ? 0 : totalWidth + Spacing * (count + 1); 
			}

		}
	}
}
