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
		public void TestThatCaretPosIsOnOneOfTheSelectionBorders ()
		{
			Position caretPos = new Position(5, 5);
			Position[] anchorPos = new Position[] {
				new Position (1,1), new Position (1,5), new Position (1, 10),
				new Position (5,1), new Position (5,5), new Position (5, 10),
				new Position (10,1), new Position (10,5), new Position (10, 10),
			};
			Selection selection = GetSelectionWithCaretPos(caretPos.Row, caretPos.Column);

			for (int i=0; i<anchorPos.Length; i++)
			{
				selection.Anchor = anchorPos[i];
				
				bool caretIsOk = selection.BeginDrawPos == caretPos || selection.EndDrawPos == caretPos;
				if (!caretIsOk)
					System.Console.WriteLine("Error: Caret is NOT on selection border" + " \nData: " + selection + ", Index: " + i);

				bool anchorIsOk = selection.BeginDrawPos == selection.Anchor || selection.EndDrawPos == selection.Anchor;
				if (!anchorIsOk)
					System.Console.WriteLine("Error: Anchor is NOT on selection border" + " \nData: " + selection + ", Index: " + i);

				Assert.IsTrue(caretIsOk);
				Assert.IsTrue(anchorIsOk);
			}
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
