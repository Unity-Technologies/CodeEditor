namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface IDefaultTextViewMarginsProvider
	{
		ITextViewMargins MarginsFor(ITextView textView);
	}
}