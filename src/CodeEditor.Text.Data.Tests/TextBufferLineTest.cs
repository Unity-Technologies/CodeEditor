using System.Linq;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextBufferLineTest : TextBufferBasedTest
	{
		[Test]
		[TestCaseAttribute(0, 0, 7)]
		[TestCaseAttribute(1, 7, 6)]
		public void ExtentIncludingLineBreak(int lineNumber, int expectedStart, int expectedLength)
		{
			SetText("line 0\nline 1");
			Assert.AreEqual(new Span(expectedStart, expectedLength), Buffer.CurrentSnapshot.Lines[lineNumber].ExtentIncludingLineBreak.Span);
		}

		[Test]
		[TestCaseAttribute("0", new[] { "0" })]
		[TestCaseAttribute("0\n", new[] { "0", "" })]
		[TestCaseAttribute("0\n1", new[] { "0", "1" })]
		[TestCaseAttribute("0\n1\n", new[] { "0", "1", "" })]
		public void InsertingNewLineCharacterBegetsNewLine(string textToBeInserted, string[] expectedLines)
		{
			SetText("");
			Buffer.Insert(0, textToBeInserted);
			Assert.AreEqual(expectedLines, Buffer.CurrentSnapshot.Lines.Select(l => l.Text).ToArray());
		}
	}
}
