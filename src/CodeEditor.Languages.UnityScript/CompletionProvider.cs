using System.Linq;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.Languages.UnityScript.ContentType;
using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;
using CodeEditor.Text.UI.Completion.Implementation;

namespace CodeEditor.Languages.UnityScript
{
	[Export(typeof(ICompletionProvider))]
	[ContentType(UnityScriptContentType.Name)]
	public class UnityScriptCompletionProvider : ICompletionProvider
	{
		public ICompletionSet CompletionsFor(TextSpan contextForCompletion)
		{
			return new CompletionSet(UnityScriptClassifier.Keywords.Select(kw => (ICompletion)new Completion(kw)));
		}
	}
}
