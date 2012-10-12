using System.Collections.Generic;
using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI.Completion
{
	public interface ICompletionProvider
	{
		ICompletionSet CompletionsFor(TextSpan contextForCompletion);
	}

	public interface ICompletionSet
	{
		IEnumerable<ICompletion> Completions { get; }
		bool IsEmpty { get; }
	}

	public interface ICompletion
	{
		string DisplayText { get; }
		string Tooltip { get; }
	}
}