using System;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextBufferLinesTest : TextBufferBasedTest
	{
		[SetUp]
		public void SetUp()
		{
			//       0123456 789012
			SetText("line 0\nline 1");
		}

		[Test]
		public void LineCount()
		{
			Assert.AreEqual(2, CurrentSnapshot.Lines.Count);
		}

		[Test]
		public void LineTextDoesNotIncludeLineBreak()
		{
			AssertTextOfLine(0, "line 0");
			AssertTextOfLine(1, "line 1");
		}

		[Test]
		[TestCase(0, 0)]
		[TestCase(5, 0)]
		[TestCase(6, 0)]
		[TestCase(7, 1)]
		[TestCase(12, 1)]
		[TestCase(13, 1)]
		public void LineForPosition(int position, int expectedLine)
		{
			Assert.AreEqual(expectedLine, CurrentSnapshot.LineNumberForPosition(position));
		}

		[Test]
		[TestCase(-1)]
		[TestCase(2)]
		public void LineAccessorThrowsOnInvalidNumber(int invalidLineNumber)
		{
			Assert.Throws<ArgumentOutOfRangeException>(delegate { var invalid = CurrentSnapshot.Lines[invalidLineNumber]; });
		}
	}
}
