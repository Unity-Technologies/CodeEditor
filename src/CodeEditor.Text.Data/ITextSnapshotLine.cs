namespace CodeEditor.Text.Data
{
	public interface ITextSnapshotLine
	{
		ITextSnapshot Snapshot { get; }
		string Text { get; }
		TextSpan Extent { get; }
		TextSpan ExtentIncludingLineBreak { get; }
		int Start { get; }
		int End { get; }
		int Length { get; }
		int LineNumber { get; }
	}
}
