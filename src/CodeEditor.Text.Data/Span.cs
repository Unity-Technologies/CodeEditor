using System;

namespace CodeEditor.Text.Data
{
	public struct Span
	{
		public readonly int Start;
		public readonly int Length;

		public Span(int start, int length)
		{
			Start = start;
			Length = length;
		}

		public int End
		{
			get { return Start + Length; }
		}

		public bool Contains(int position)
		{
			return position >= Start && position < End;
		}

		public override string ToString()
		{
			return string.Format("[{0}..{1})", Start, End);
		}

		public static Span operator +(Span x, Span y)
		{
			if (x.End != y.Start)
				throw new ArgumentException("Can not add non contiguous Spans");

			return new Span(x.Start, x.Length + y.Length);
		}
	}
}
