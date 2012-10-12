using CodeEditor.Text.Data.Implementation;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	public class TextBufferBasedTest
	{
		private ITextBuffer _buffer;

		[SetUp]
		public void CreateBuffer()
		{
			SetText("");
		}

		protected ITextBuffer Buffer
		{
			get { return _buffer; }
		}

		protected ITextSnapshot CurrentSnapshot
		{
			get { return Buffer.CurrentSnapshot; }
		}

		protected void AssertTextOfLine(int lineNumber, string expected)
		{
			Assert.AreEqual(expected, Buffer.CurrentSnapshot.Lines[lineNumber].Text);
		}

		protected void SetText(string text)
		{
			_buffer = new TextBuffer(text, null);
		}

		protected void AssertText(string expected)
		{
			Assert.AreEqual(expected, Buffer.CurrentSnapshot.Text);
		}

		protected ITextSnapshotLine TextLine(int number)
		{
			return Buffer.CurrentSnapshot.Lines[number];
		}
	}
}
