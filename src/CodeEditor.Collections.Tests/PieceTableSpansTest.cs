using NUnit.Framework;

namespace CodeEditor.Collections.Tests
{
	[TestFixture]
	public class PieceTableSpansTest
	{
		[Test]
		[TestCase("", 0)]
		[TestCase("01", 0)]
		[TestCase("01", 1)]
		[TestCase("01", 2)]
		public void InsertingAdjacentElementDoesNotCausePieceTableGrowth(string s, int index)
		{
			var v0 = PieceTable.ForString(s);
			var v1 = v0.Insert(index, '*');
			var v2 = v1.Insert(index + 1, '-');
			Assert.AreEqual(v1.PieceCount, v2.PieceCount);
		}

		[Test]
		[TestCase(0, 3, 0)]
		[TestCase(0, 1, 2)]
		[TestCase(0, 2, 1)]
		[TestCase(1, 1, 2)]
		[TestCase(1, 2, 1)]
		[TestCase(2, 1, 2)]
		public void DeletionPieceCountAfterInsert(int position, int length, int expectedPieceCount)
		{
			var v0 = PieceTable.ForString("02");
			var v1 = v0.Insert(1, '1');
			Assert.AreEqual(3, v1.PieceCount);

			var v2 = v1.Delete(position, length);
			Assert.AreEqual(expectedPieceCount, v2.PieceCount);
		}
	}
}
