using System.Linq;
using CodeEditor.Languages.UnityScript;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Tests;
using CodeEditor.Text.Logic;
using CodeEditor.Text.Logic.Implementation;
using NUnit.Framework;

namespace CodeEditor.Languages.Tests
{
	[TestFixture]
	public class UnityScriptClassifierTest : TextBufferBasedTest
	{
		private UnityScriptClassifier _unityScriptClassifier;
		private readonly IStandardClassificationRegistry _standardClassificationRegistry = new StandardClassificationRegistry();

		[SetUp]
		public void SetUp()
		{
			_unityScriptClassifier = new UnityScriptClassifier(ClassificationRegistry, Buffer);
		}

		[Test]
		public void ClassifyNumber()
		{
			Buffer.Insert(0, "43.99");
			AssertClassificationsOf(TextLine(0), Number(0, 5));
		}

		[Test]
		public void ClassifyComplexCodeSnippet()
		{
			SetText("function foo() { var s = 's'; }");
			var expected = new[]
			{
				Keyword(0, 8),
				WhiteSpace(8, 1),
				Identifier(9, 3),
				Punctuation(12, 2),
				WhiteSpace(14, 1),
				Punctuation(15, 1),
				WhiteSpace(16, 1),
				Keyword(17, 3),
				WhiteSpace(20, 1),
				Identifier(21, 1),
				WhiteSpace(22, 1),
				Operator(23, 1),
				WhiteSpace(24, 1),
				String(25, 3),
				Punctuation(28, 1),
				WhiteSpace(29, 1),
				Punctuation(30, 1)
			};
			AssertClassificationsOf(TextLine(0), expected);
		}

		[Test]
		public void SimpleComment()
		{
			SetText("// hello\nfoo");
			AssertClassificationsOf(TextLine(0), Comment(TextLine(0).Extent));
			AssertClassificationsOf(TextLine(1), Identifier(TextLine(1).Extent));
		}

		[Test]
		public void ClassifyEmptyLine()
		{
			SetText("\n");
			AssertClassificationsOf(TextLine(0));
		}

		[Test]
		public void ClassifyLineWhenPreviousLineIsEmpty()
		{
			SetText("\nfoo");
			AssertClassificationsOf(TextLine(1), Identifier(1, 3));
		}

		[Test]
		public void ClassifyLineWhenPreviousLineEndsInLineComment()
		{
			SetText("//\nfoo");
			AssertClassificationsOf(TextLine(1), Identifier(3, 3));
		}

		[Test]
		public void BlockComment()
		{
			SetText("/* hello \nbye */");
			AssertClassificationsOf(TextLine(0), Comment(TextLine(0).Extent));
			AssertClassificationsOf(TextLine(1), Comment(TextLine(1).Extent));
		}

		[Test]
		public void ClassifyLineWithoutClassifyingPreviousLine()
		{
			SetText("/* hello \nbye */");
			AssertClassificationsOf(TextLine(1), Comment(TextLine(1).Extent));
		}

		[Test]
		public void ReclassifyAfterInsertInPreviousLine()
		{
			Buffer.Insert(0, "* hello \nbye */");
			Classify(TextLine(1));
			Buffer.Insert(0, "/");
			AssertClassificationsOf(TextLine(1), Comment(TextLine(1).Extent));
		}

		private ClassificationSpan Comment(TextSpan textSpan)
		{
			return new ClassificationSpan(ClassificationRegistry.Comment, textSpan);
		}

		private ClassificationSpan Number(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.Number);
		}

		private ClassificationSpan Identifier(TextSpan textSpan)
		{
			return Identifier(textSpan.Span.Start, textSpan.Span.Length);
		}

		private ClassificationSpan Keyword(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.Keyword);
		}

		private ClassificationSpan WhiteSpace(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.WhiteSpace);
		}

		private ClassificationSpan Identifier(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.Identifier);
		}

		private ClassificationSpan Punctuation(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.Punctuation);
		}

		private ClassificationSpan Operator(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.Operator);
		}

		private ClassificationSpan String(int position, int length)
		{
			return ClassificationSpan(position, length, ClassificationRegistry.String);
		}

		private IStandardClassificationRegistry ClassificationRegistry
		{
			get { return _standardClassificationRegistry; }
		}

		private ClassificationSpan ClassificationSpan(int position, int length, IClassification classification)
		{
			return new ClassificationSpan(classification, Buffer.CurrentSnapshot.GetSpan(position, length));
		}

		private void AssertClassificationsOf(ITextSnapshotLine line, params ClassificationSpan[] expected)
		{
			CollectionAssert.AreEqual(expected, Classify(line));
		}

		private ClassificationSpan[] Classify(ITextSnapshotLine line)
		{
			return _unityScriptClassifier.Classify(line).ToArray();
		}
	}
}
