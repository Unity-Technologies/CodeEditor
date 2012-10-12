using System.Collections.Generic;
using System.Linq;

namespace CodeEditor.Text.UI.Completion.Implementation
{
	public class CompletionSet : ICompletionSet
	{
		private readonly ICompletion[] _completions;

		public CompletionSet(IEnumerable<ICompletion> completions)
		{
			_completions = completions.ToArray();
		}

		public IEnumerable<ICompletion> Completions
		{
			get { return _completions; }
		}

		public bool IsEmpty
		{
			get { return _completions.Length == 0; }
		}
	}
}
