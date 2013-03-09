using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.CSharp
{
	[Export(typeof(IClassifierProvider))]
	[ContentType(CSharpContentType.Name)]
	class CSharpClassifierProvider : IClassifierProvider
	{
		[Import]
		public IStandardClassificationRegistry ClassificationRegistry { get; set; }

		public IClassifier ClassifierFor(ITextBuffer buffer)
		{
			return new CSharpClassifier(ClassificationRegistry, buffer);
		}
	}
}