using System.Collections.Generic;
using System.Linq;
using CodeEditor.IO;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewDocument : ITextViewDocument
	{
		public ICaret Caret { get; private set; }

		private readonly ITextDocument _document;
		private readonly IClassifier _classifier;
		private readonly IClassificationStyler _classificationStyler;
		private readonly Dictionary<int, ITextViewLine> _cachedLines = new Dictionary<int, ITextViewLine>();

		public TextViewDocument(ITextDocument document, ICaret caret, IClassifier classifier, IClassificationStyler classificationStyler)
		{
			_document = document;
			_classifier = classifier;
			_classificationStyler = classificationStyler;
			Buffer.Changed += OnBufferChange;
			Caret = caret;
		}

		void OnBufferChange(object sender, TextChangeArgs args)
		{
			RemoveCachedLinesFrom(args.LineNumber);
		}

		private void RemoveCachedLinesFrom(int lineNumber)
		{
			RemoveLines(_cachedLines.Keys.Where(k => k >= lineNumber).ToArray());
		}

		private void RemoveLines(int[] keys)
		{
			foreach (var key in keys)
				_cachedLines.Remove(key);
		}

		public ITextBuffer Buffer
		{
			get { return _document.Buffer; }
		}

		public ITextViewLine CurrentLine
		{
			get { return Line(Caret.Row); }
		}

		public IFile File
		{
			get { return _document.File; }
		}

		public ITextViewLine Line(int index)
		{
			ITextViewLine cached;
			if (_cachedLines.TryGetValue(index, out cached))
				return cached;

			var newLine = new TextViewLine(this, Buffer.CurrentSnapshot.Lines[index]);
			_cachedLines.Add(index, newLine);
			return newLine;
		}

		public void Save()
		{
			_document.Save();
		}

		public int LineCount
		{
			get { return Buffer.CurrentSnapshot.Lines.Count; }
		}

		public IEnumerable<ClassificationSpan> Classify(ITextSnapshotLine textLine)
		{
			return _classifier.Classify(textLine);
		}

		public Color ColorFor(IClassification classification)
		{
			return _classificationStyler.ColorFor(classification);
		}
	}
}
