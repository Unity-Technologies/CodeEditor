using CodeEditor.Testing;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Implementation;
using CodeEditor.Text.Logic;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Unity.Engine.Tests
{
	[TestFixture]
	public class TextViewDocumentTest : MockBasedTest
	{
		private ITextViewDocument _document;

		[SetUp]
		public void SetUp()
		{
			_document = TextViewDocumentFor("\nfunction Update() {\tprint('foo'); }");
		}

		[Test]
		public void RemovingBeforeFirstCharacterRemovesLine()
		{
			Assert.AreEqual(2, _document.LineCount);
			_document.Delete(_document.Line(1).Start - 1, 1);
			Assert.AreEqual(1, _document.LineCount);
		}

		[Test]
		public void RemovingLastCharacterRemovesLine()
		{
			_document.Delete(_document.Line(0).Start, 1);
			Assert.AreEqual(1, _document.LineCount);
		}

		[Test]
		public void RemoveUpdatesText()
		{
			var oldText = _document.Line(1).Text;
			_document.Delete(_document.Line(1).Start, 1);
			Assert.AreEqual(oldText.Substring(1), _document.Line(1).Text);
		}

		[Test]
		[TestCaseAttribute(0, 0, "function Update() {\tprint('foo'); }")]
		[TestCaseAttribute(1, 0, "")]
		public void DeleteLine(int lineToDelete, int expectedLine, string expectedText)
		{
			_document.DeleteLine(lineToDelete);
			Assert.AreEqual(_document.Line(expectedLine).Text, expectedText);
		}

		private ITextViewDocument TextViewDocumentFor(string content)
		{
			var document = MockFor<ITextDocument>();
			document.SetupGet(_ => _.Buffer).Returns(new TextBuffer(content, null));

			return new TextViewDocument(document.Object, MockFor<ICaret>().Object, MockFor<IClassifier>().Object, MockFor<IClassificationStyler>().Object);
		}
	}
}
