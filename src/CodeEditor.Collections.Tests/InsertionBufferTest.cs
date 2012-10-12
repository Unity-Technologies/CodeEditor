using NUnit.Framework;

namespace CodeEditor.Collections.Tests
{
	[TestFixture]
	public class InsertionBufferTest
	{
		[Test]
		public void ReturnedPieceIsAppendableWhileBufferHasntChanged()
		{
			var buffer = new InsertionBuffer<int>();

			var piece = buffer.Insert(42);
			Assert.IsTrue(IsAppendable(piece));
			Assert.IsTrue(IsAppendable(piece.Append(42)));
		}

		[Test]
		public void ReturnedPieceIsNoLongerAppendableAfterBufferHasChanged()
		{
			var buffer = new InsertionBuffer<int>();

			var piece = buffer.Insert(42);
			Assert.IsTrue(IsAppendable(piece));

			buffer.Insert(42);
			Assert.IsFalse(IsAppendable(piece));
		}

		[Test]
		[TestCase(4, 0, 0, 4, new[] { 1, 2, 3, 4 })]
		[TestCase(2, 0, 0, 2, new[] { 1, 2 })]
		[TestCase(1, 0, 0, 1, new[] { 1 })]
		[TestCase(4, 1, 1, 3, new[] { 0, 2, 3, 4 })]
		[TestCase(3, 1, 0, 2, new[] { 0, 1, 2 })]
		[TestCase(2, 0, 1, 1, new[] { 2, 0 })]
		public void CopyTo(int destinationLength, int destinationIndex, int sourceIndex, int length, int[] expected)
		{
			var buffer = new InsertionBuffer<int>();
			buffer.Insert(42); // some recognizable "garbage" at the beginning of the buffer

			// 1234
			var piece = buffer.Insert(1);
			for (var i = 2; i < 5; ++i)
				piece = (IAppendablePiece<int>) piece.Append(i);

			var destination = new int[destinationLength];
			piece.CopyTo(destination, destinationIndex, sourceIndex, length);
			Assert.AreEqual(expected, destination);
		}

		private static bool IsAppendable(IPiece<int> piece)
		{
			return Piece.IsAppendable(piece);
		}
	}
}
