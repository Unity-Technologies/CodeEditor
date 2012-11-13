using CodeEditor.Text.UI.Unity.Engine.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Unity.Engine.Tests
{
	[TestFixture]
	public class TextViewSelectionTest
	{
		TextViewSelection GetSelectionWithCaretPos (int row, int column)
		{
			var documentMock = new Mock<ITextViewDocument>();
			documentMock.SetupGet(o => o.Caret.Row).Returns(row);
			documentMock.SetupGet(o => o.Caret.Column).Returns(column);
			return new TextViewSelection(documentMock.Object);
		}

		void CheckValidBeginAndEndDrawPositions(TextViewPosition beginDrawPos, TextViewPosition endDrawPos)
		{
			// We draw from upper left to lower right
			Assert.IsTrue(beginDrawPos.row <= endDrawPos.row);
			if (beginDrawPos.row == endDrawPos.row)
				Assert.IsTrue(beginDrawPos.column < endDrawPos.column);
		}

		[Test]
		public void TestSelectionWhereCaretIsAfterAnchorOnSameRow()
		{
			TextViewSelection selection = GetSelectionWithCaretPos(5, 9);
			selection.Anchor = new TextViewPosition (5,5);	
			CheckValidBeginAndEndDrawPositions (selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretIsBeforeAnchorOnSameRow()
		{
			TextViewSelection selection = GetSelectionWithCaretPos(5, 3);
			selection.Anchor = new TextViewPosition(5, 5);
			CheckValidBeginAndEndDrawPositions (selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretRowIsBeforeAnchorRow()
		{
			TextViewSelection selection = GetSelectionWithCaretPos(2, 2);
			selection.Anchor = new TextViewPosition(6, 6);
			CheckValidBeginAndEndDrawPositions(selection.BeginDrawPos, selection.EndDrawPos);
		}

		[Test]
		public void TestSelectionWhereCaretRowIsAfterAnchorRow()
		{
			TextViewSelection selection = GetSelectionWithCaretPos(6, 6);
			selection.Anchor = new TextViewPosition(2, 2);
			CheckValidBeginAndEndDrawPositions(selection.BeginDrawPos, selection.EndDrawPos);
		}
	}
}
