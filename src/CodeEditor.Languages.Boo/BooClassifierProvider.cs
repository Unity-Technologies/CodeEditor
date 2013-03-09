using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.Boo
{
	[Export(typeof(IClassifierProvider))]
	[ContentType(BooContentType.Name)]
	class BooClassifierProvider : IClassifierProvider
	{
		[Import]
		public IStandardClassificationRegistry ClassificationRegistry { get; set; }

		public IClassifier ClassifierFor(ITextBuffer buffer)
		{
			return new BooClassifier(ClassificationRegistry, buffer);
		}
	}
}