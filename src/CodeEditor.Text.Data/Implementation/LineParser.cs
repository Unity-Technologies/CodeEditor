using System.Collections.Generic;
using CodeEditor.Collections;

namespace CodeEditor.Text.Data.Implementation
{
	public static class LineParser
	{
		public static IEnumerable<LineSpan> LineSpansFor(PieceTable<char> text)
		{
			var length = text.Length;
			if (length == 0)
			{
				yield return new LineSpan(0, 0, 0);
				yield break;
			}

			var offset = 0;
			var textLength = 0;
			var iterator = text[0];
			do
			{
				var c = iterator.GetValue();
				if (c == '\n')
				{
					const int lineBreakLength = 1;
					yield return new LineSpan(offset, textLength, lineBreakLength);
					offset += textLength + lineBreakLength;
					textLength = 0;
				}
				else
					++textLength;

				iterator = iterator.Next;
			}
			while (iterator.Position < length);

			if (textLength > 0 || iterator.Previous.GetValue() == '\n')
				yield return new LineSpan(offset, textLength, 0);
		}
	}
}