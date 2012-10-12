using System;

namespace CodeEditor.Text.Data
{
	public struct TextSpan
	{
		public readonly ITextSnapshot Snapshot;
		public Span Span;

		public TextSpan(ITextSnapshot snapshot, Span span)
		{
			Snapshot = snapshot;
			Span = span;
		}

		public TextSpan(ITextSnapshot snapshot, int start, int length)
			: this(snapshot, new Span(start, length))
		{
		}

		public string Text
		{
			get { return Snapshot.GetText(Span); }
		}

		public int Start
		{
			get { return Span.Start; }
		}

		public int Length
		{
			get { return Span.Length; }
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Span, SafeText);
		}

		private string SafeText
		{
			get
			{
				try
				{
					return Text;
				}
				catch (Exception e)
				{
					return "ERROR: " + e.Message;
				}
			}
		}

		public int End
		{
			get { return Span.End; }
		}

		public static TextSpan operator +(TextSpan x, TextSpan y)
		{
			if (x.Snapshot != y.Snapshot)
				throw new ArgumentException("Can not add TextSpan from different buffers");

			return new TextSpan(x.Snapshot, x.Span + y.Span);
		}

		public bool Contains(int position)
		{
			return Span.Contains(position);
		}
	}
}
