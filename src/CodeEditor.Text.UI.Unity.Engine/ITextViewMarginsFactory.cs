namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewMarginsFactory
	{
		ITextViewMargins MarginsFor(ITextView textView);
	}
}