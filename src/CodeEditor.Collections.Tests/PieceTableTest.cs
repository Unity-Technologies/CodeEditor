using System.Linq;
using NUnit.Framework;

namespace CodeEditor.Collections.Tests
{
	[TestFixture]
	public class PieceTableTest
	{
		[Test]
		[TestCase(1)]
		[TestCase(2)]
		public void LengthAfterAppending(int insertionCount)
		{
			var table = PieceTable.ForString("");
			for (var i = 0; i < insertionCount; ++i)
				table = table.Insert(table.Length, ' ');
			Assert.AreEqual(insertionCount, table.Length);
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		public void LengthAfterPrepending(int insertionCount)
		{
			var table = PieceTable.ForString("");
			for (var i = 0; i < insertionCount; ++i)
				table = table.Insert(0, ' ');
			Assert.AreEqual(insertionCount, table.Length);
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		public void LengthAfterSplitting(int insertionCount)
		{
			const string originalString = "**";
			var table = PieceTable.ForString(originalString);
			for (var i = 0; i < insertionCount; ++i)
				table = table.Insert(1, ' ');
			Assert.AreEqual(insertionCount + originalString.Length, table.Length);
		}

		[Test]
		[TestCase(1)]
		[TestCase(2)]
		public void LengthAfterSplittingAndAppending(int insertionCount)
		{
			const string originalString = "**";
			var table = PieceTable.ForString(originalString);
			for (var i = 0; i < insertionCount; ++i)
				table = table.Insert(1, ' ').Insert(2, ' ');
			Assert.AreEqual(insertionCount * 2 + originalString.Length, table.Length);
		}

		[Test]
		public void InsertPreservesOriginalVersion()
		{
			const string originalString = "01234567";
			var v0 = PieceTable.ForString(originalString);
			var v1 = v0.Insert(2, '*');
			Assert.AreEqual(originalString.Insert(2, "*"), StringFor(v1));
			Assert.AreEqual(originalString, StringFor(v0));
		}

		[Test]
		[TestCase(0, "")]
		[TestCase(0, "01")]
		[TestCase(1, "01")]
		[TestCase(2, "01")]
		public void InsertFollowedByInsert(int index, string s)
		{
			var v0 = PieceTable.ForString(s);
			var v1 = v0.Insert(index, '*');
			var v2 = v1.Insert(index + 1, '-');
			Assert.AreEqual(s.Insert(index, "*-"), StringFor(v2));
		}

		[Test]
		[TestCase(0, 0, "")]
		[TestCase(0, 1, "0")]
		[TestCase(0, 2, "01")]
		[TestCase(1, 1, "1")]
		[TestCase(1, 2, "1*")]
		[TestCase(1, 3, "1*2")]
		[TestCase(6, 4, "5*67")]
		public void GetRangeAfterMultipleInserts(int position, int length, string expectedString)
		{
			const string originalString = "01234567";
			var v0 = PieceTable.ForString(originalString);
			var v1 = v0.Insert(2, '*');
			var v2 = v1.Insert(v1.Length - 2, '*');
			Assert.AreEqual(expectedString, new string(v2.ToArray(position, length)));
		}

		[Test]
		[TestCase(0, 1, "123")]
		[TestCase(0, 2, "23")]
		[TestCase(1, 1, "023")]
		[TestCase(1, 2, "03")]
		[TestCase(3, 1, "012")]
		[TestCase(0, 4, "")]
		public void Delete(int position, int length, string expectedString)
		{
			const string originalString = "0123";
			var v0 = PieceTable.ForString(originalString);
			var v1 = v0.Delete(position, length);
			Assert.AreEqual(expectedString, StringFor(v1));
		}

		[Test]
		[TestCase(0, 1, "123")]
		[TestCase(0, 2, "23")]
		[TestCase(1, 1, "023")]
		[TestCase(1, 2, "03")]
		[TestCase(3, 1, "012")]
		[TestCase(0, 4, "")]
		public void DeleteAfterInsert(int position, int length, string expectedString)
		{
			const string originalString = "023";
			var v0 = PieceTable.ForString(originalString);
			var v1 = v0.Insert(1, '1');
			var v2 = v1.Delete(position, length);
			Assert.AreEqual(expectedString, StringFor(v2));
		}

		[Test]
		public void InsertMany()
		{
			var table = CreateTableWithManyPieces(128);
			Assert.AreEqual(Enumerable.Range(0, 128).Reverse().ToArray(), table.ToArray());
		}

		[Test]
		public void PointBackwardsIteration()
		{
			var table = CreateTableWithManyPieces(128);
			var point = table[table.Length - 1];
			for (var i = 0; i < 128; ++i)
			{
				Assert.AreEqual(i, point.GetValue());
				point = point.Previous;
			}
		}

		[Test]
		public void PointFowardIteration()
		{
			var table = CreateTableWithManyPieces(128);
			var point = table[0];
			for (var i = 128; i > 0; --i)
			{
				Assert.AreEqual(i - 1, point.GetValue());
				point = point.Next;
			}
		}

		[Test]
		public void PreviousPointOfEndPoint()
		{
			var table = PieceTable.ForString("0");
			var pointAfterLast = table[0].Next;
			Assert.AreEqual('0', pointAfterLast.Previous.GetValue());
		}

		private static PieceTable<int> CreateTableWithManyPieces(int length)
		{
			var table = PieceTable.ForArray(new int[0]);
			for (var i = 0; i < length; ++i) table = table.Insert(0, i);
			return table;
		}

		private static string StringFor(PieceTable<char> p)
		{
			return new string(p.ToArray());
		}
	}
}
