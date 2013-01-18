using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;
using CodeEditor.Text.UI.Completion.Implementation;

namespace CodeEditor.Languages.CSharp
{
	public class CSharpCompletionProvider : ICompletionProvider
	{
		public ICompletionSet CompletionsFor(TextSpan contextForCompletion)
		{
			return new CompletionSet(new Completion[0]);
		}
	}
}
