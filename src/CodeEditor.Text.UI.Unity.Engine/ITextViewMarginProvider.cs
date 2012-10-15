namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewMarginProvider
	{
		ITextViewMargin MarginFor(ITextView textView);
	}
}