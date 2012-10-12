namespace CodeEditor.Text.Data.Implementation
{
	public struct LineSpan
	{
		public readonly int Start;
		public readonly int TextLength;
		public readonly int LineBreakLength;

		public LineSpan(int start, int textLength, int lineBreakLength)
		{
			Start = start;
			TextLength = textLength;
			LineBreakLength = lineBreakLength;
		}

		public int LengthIncludingLineBreak
		{
			get { return TextLength + LineBreakLength; }
		}

		public int End
		{
			get { return Start + LengthIncludingLineBreak; }
		}

		public bool IsTerminated
		{
			get { return LineBreakLength > 0; }
		}

		public bool Contains(int position)
		{
			return position >= Start && position < End;
		}
	}
}