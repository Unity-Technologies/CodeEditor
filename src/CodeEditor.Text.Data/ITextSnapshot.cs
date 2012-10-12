namespace CodeEditor.Text.Data
{
	public interface ITextSnapshot
	{
		char this[int position] { get; }
		string Text { get; }
		int Length { get; }
		ITextBuffer Buffer { get; }
		ITextSnapshotLines Lines { get; }

		string GetText(Span span);
		TextSpan GetSpan(int position, int length);
		int LineNumberForPosition(int position);
		string GetText(int start, int length);
	}

	public static class TextSnapshotExtensions
	{
		public static TextSpan GetSpan(this ITextSnapshot snapshot, Span span)
		{
			return snapshot.GetSpan(span.Start, span.Length);
		}
	}
}
