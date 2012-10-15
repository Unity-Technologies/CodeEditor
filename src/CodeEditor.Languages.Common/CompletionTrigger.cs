using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;

namespace CodeEditor.Languages.Common
{
	public class CompletionTrigger : ICompletionTrigger
	{
		private readonly ITextBuffer _buffer;
		private readonly ICompletionProvider _completionProvider;
		private readonly ICompletionSessionProvider _completionSessionProvider;

		public CompletionTrigger(ITextBuffer buffer, ICompletionProvider completionProvider, ICompletionSessionProvider completionSessionProvider)
		{
			_buffer = buffer;
			_completionProvider = completionProvider;
			_completionSessionProvider = completionSessionProvider;
			_buffer.Changed += OnBufferChanged;
		}

		void OnBufferChanged(object sender, TextChangeArgs args)
		{
			if (!(args.NewText == "t" || args.NewText == "."))
				return;

			var completions = CompletionsFor(args.NewTextSpan);
			if (completions.IsEmpty)
				return;

			_completionSessionProvider.StartCompletionSession(args.NewTextSpan, completions);
		}

		private ICompletionSet CompletionsFor(TextSpan newTextSpan)
		{
			return _completionProvider.CompletionsFor(newTextSpan);
		}
	}
}