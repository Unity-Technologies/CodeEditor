using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI
{
	public interface ITextStructureNavigator
	{
		TextSpan GetSpanFor(int position, ITextSnapshot snapshot);
		TextSpan GetNextSpanFor(TextSpan span);
		TextSpan GetPreviousSpanFor(TextSpan span);
	}
}
