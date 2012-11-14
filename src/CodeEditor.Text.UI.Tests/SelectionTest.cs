using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.UI
{
	[TestFixture]
	public class SelectionTest
	{
		Selection GetSelectionWithCaretPos (int row, int column)
		{
			var documentMock = new Mock<ICaret>();
			documentMock.SetupGet(o => o.Row).Returns(row);
			documentMock.SetupGet(o => o.Column).Returns(column);
			return new Selection(documentMock.Object);
		}

		void CheckValidBeginAndEndDrawPositions(Position beginDrawPos, Position endDrawPos)
		{
			// We draw from upper left to lower right
			Assert.IsTrue(beginDrawPos.Row <= endDrawPos.Row);
			if (beginDrawPos.Row == endDrawPos.Row)
				Assert.IsTrue(beginDrawPos.Column < endDrawPos.Column);
		}

		[Test]
		public void TestSelectionWhereCaretIsAfterAnchorOnSameRow()
		{
			Selection selection = GetSelectionWithCaretPos(5, 9);
			selection.Anchor = new Position (5,5);	
			CheckValidBeginAndEndDrawPositions (selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretIsBeforeAnchorOnSameRow()
		{
			Selection selection = GetSelectionWithCaretPos(5, 3);
			selection.Anchor = new Position(5, 5);
			CheckValidBeginAndEndDrawPositions (selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretRowIsBeforeAnchorRow()
		{
			Selection selection = GetSelectionWithCaretPos(2, 2);
			selection.Anchor = new Position(6, 6);
			CheckValidBeginAndEndDrawPositions(selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretRowIsAfterAnchorRow()
		{
			Selection selection = GetSelectionWithCaretPos(6, 6);
			selection.Anchor = new Position(2, 2);
			CheckValidBeginAndEndDrawPositions(selection.BeginDrawPos, selection.EndDrawPos);
		}
	}
}
