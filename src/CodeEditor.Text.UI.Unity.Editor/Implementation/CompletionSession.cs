using System.Collections.Generic;
using System.Linq;
using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CompletionSession
	{
		private readonly ICompletionSet _completions;

		public CompletionSession(TextSpan completionSpan, ICompletionSet completions)
		{
			_completions = completions;
		}

		public IEnumerable<string> Completions
		{
			get { return _completions.Completions.Select(c => c.DisplayText); }
		}
	}
}