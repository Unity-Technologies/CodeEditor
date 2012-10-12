using System;
using System.Diagnostics;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class EmptyTextBufferTest : TextBufferBasedTest
	{
		[Test]
		public void EmptyBufferHasSingleEmptyLine()
		{
			Assert.AreEqual(1, Buffer.CurrentSnapshot.Lines.Count);
			AssertTextOfLine(0, "");
		}
	}
}
