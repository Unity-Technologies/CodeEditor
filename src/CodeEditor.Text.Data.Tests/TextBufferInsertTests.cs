using System;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextBufferInsertionTest : TextBufferBasedTest
	{
		[Test]
		public void Insert()
		{
			SetText("line 0");
			Buffer.Insert(0, "Foo");
			AssertTextOfLine(0, "Fooline 0");
		}

		[Test]
		public void InsertNormalizesLineTerminators()
		{
			Buffer.Insert(0, "\r\n");
			AssertText("\n");
		}

		[Test]
		public void InsertCreatesNewSnapshot()
		{
			var snapshot = Buffer.CurrentSnapshot;
			Assert.AreEqual(0, snapshot.Lines[0].Length);
			Buffer.Insert(0, "a");
			Assert.AreEqual(1, Buffer.CurrentSnapshot.Lines[0].Length);
			Assert.AreEqual(0, snapshot.Lines[0].Length);
		}

		[Test]
		public void InsertTriggersChangedEvent()
		{
			SetText("foo\n");
			TextChangeArgs changedArgs = null;
			Buffer.Changed += (sender, args) => changedArgs = args;
			const string expected = "bar";
			Buffer.Insert(4, expected);
			Assert.AreEqual(expected, changedArgs.NewText);
			Assert.AreEqual(4, changedArgs.NewSpan.Start);
			Assert.AreEqual(3, changedArgs.NewSpan.Length);
		}

		[Test]
		public void InsertEmptyStringDoesNotTriggerChangedEvent()
		{
			Buffer.Changed += (sender, args) => Assert.Fail();
			Buffer.Insert(0, "");
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InsertNullThrows()
		{
			Buffer.Insert(0, null);
		}
	}
}
