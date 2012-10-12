namespace CodeEditor.Text.Data.Implementation
{
	class TextSnapshotLine : ITextSnapshotLine
	{
		private readonly ITextSnapshot _snapshot;
		private readonly LineSpan _lineSpan;
		private readonly int _lineNumber;

		public TextSnapshotLine(ITextSnapshot snapshot, LineSpan lineSpan, int lineNumber)
		{
			_snapshot = snapshot;
			_lineSpan = lineSpan;
			_lineNumber = lineNumber;
		}

		public ITextSnapshot Snapshot
		{
			get { return _snapshot; }
		}

		public string Text
		{
			get { return Extent.Text; }
		}

		public TextSpan ExtentIncludingLineBreak
		{
			get { return Snapshot.GetSpan(Start, LengthIncludingLineBreak); }
		}

		private int LengthIncludingLineBreak
		{
			get { return _lineSpan.LengthIncludingLineBreak; }
		}

		public int Start
		{
			get { return _lineSpan.Start; }
		}

		public int Length
		{
			get { return _lineSpan.TextLength; }
		}

		public int LineNumber
		{
			get { return _lineNumber; }
		}

		public TextSpan Extent
		{
			get { return Snapshot.GetSpan(Start, Length); }
		}

		public int End
		{
			get { return _lineSpan.End; }
		}

		public override string ToString()
		{
			return ExtentIncludingLineBreak.ToString();
		}
	}
}
