using CodeEditor.Composition;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.UnityScript
{
	[Export(typeof(IClassifierProvider))]
	[ContentType(UnityScriptContentType.Name)]
	class UnityScriptClassifierProvider : IClassifierProvider
	{
		[Import]
		public IStandardClassificationRegistry ClassificationRegistry { get; set; }

		public IClassifier ClassifierFor(ITextBuffer buffer)
		{
			return new UnityScriptClassifier(ClassificationRegistry, buffer);
		}
	}
}