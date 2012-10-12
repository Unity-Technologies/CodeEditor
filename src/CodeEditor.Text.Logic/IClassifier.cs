using System.Collections.Generic;
using CodeEditor.Text.Data;

namespace CodeEditor.Text.Logic
{
	public interface IClassifierProvider
	{
		IClassifier ClassifierFor(ITextBuffer buffer);
	}

	public interface IClassifier
	{
		IEnumerable<ClassificationSpan> Classify(ITextSnapshotLine line);
	}
}
