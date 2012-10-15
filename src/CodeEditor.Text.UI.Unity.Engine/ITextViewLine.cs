namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewLine
	{
		string Text { get; }
		string RichText { get; }
		int Start { get; }
		int LineNumber { get; }
	}
}
