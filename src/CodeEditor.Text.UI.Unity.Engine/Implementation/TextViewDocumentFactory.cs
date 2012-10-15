using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewDocumentFactory))]
	public class TextViewDocumentFactory : ITextViewDocumentFactory
	{
		[Import]
		public ITextDocumentFactory TextDocumentFactory { get; set; }

		[Import]
		public IClassificationStyler ClassificationStyler { get; set; }

		[Import]
		public IStandardClassificationRegistry StandardClassificationRegistry { get; set; }

		[Import]
		public ICaretFactory CaretFactory { get; set; }

		public ITextViewDocument DocumentForFile(IFile file)
		{
			var document = TextDocumentFactory.ForFile(file);
			var classifier = ClassifierFor(document.Buffer);
			var caret = CaretFactory.CaretForBuffer(document.Buffer);
			return new TextViewDocument(document, caret, classifier, ClassificationStyler);
		}

		private IClassifier ClassifierFor(ITextBuffer textBuffer)
		{
			var classifierProvider = textBuffer.ContentType.GetService<IClassifierProvider>();
			if (classifierProvider != null)
				return classifierProvider.ClassifierFor(textBuffer);
			return new TextClassifier(StandardClassificationRegistry);
		}

		private class TextClassifier : IClassifier
		{
			private readonly IStandardClassificationRegistry _standardClassificationRegistry;

			public TextClassifier(IStandardClassificationRegistry standardClassificationRegistry)
			{
				_standardClassificationRegistry = standardClassificationRegistry;
			}

			public IEnumerable<ClassificationSpan> Classify(ITextSnapshotLine line)
			{
				yield return new ClassificationSpan(_standardClassificationRegistry.Text, line.Extent);
			}
		}
	}
}
