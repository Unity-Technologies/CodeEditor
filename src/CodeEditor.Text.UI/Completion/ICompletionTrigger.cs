using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI.Completion
{
	public interface ICompletionTrigger
	{
	}

	public interface ICompletionSessionProvider
	{
		void StartCompletionSession(TextSpan completionSpan, ICompletionSet completions);
	}
}