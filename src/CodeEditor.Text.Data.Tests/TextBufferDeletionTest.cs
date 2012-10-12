using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextBufferDeletionTest : TextBufferBasedTest
	{
		[SetUp]
		public void SetUp()
		{
			SetText("foo\nbar");
		}

		[Test]
		public void DeleteFirst()
		{
			Buffer.Delete(0, 1);
			AssertText("oo\nbar");
		}

		[Test]
		public void DeleteCreatesNewSnapshot()
		{
			var snapshot = Buffer.CurrentSnapshot;
			Assert.AreEqual(3, snapshot.Lines[0].Length);
			Buffer.Delete(2, 1);
			Assert.AreEqual(2, Buffer.CurrentSnapshot.Lines[0].Length);
			Assert.AreEqual(3, snapshot.Lines[0].Length);
		}

		[Test]
		public void DeleteTriggersChangedEvent()
		{
			TextChangeArgs changedArgs = null;
			Buffer.Changed += (sender, args) => changedArgs = args;
			Buffer.Delete(0, 1);
			Assert.AreEqual("f", changedArgs.OldText);
		}

		[Test]
		public void DeleteWithZeroLengthDoesNotTriggerChangedEvent()
		{
			Buffer.Changed += (sender, args) => Assert.Fail();
			Buffer.Delete(0, 0);
		}

		[Test]
		public void DeleteLineTerminator()
		{
			Buffer.Delete(3, 1);
			Assert.AreEqual(1, Buffer.CurrentSnapshot.Lines.Count);
			AssertTextOfLine(0, "foobar");
		}
	}
}
