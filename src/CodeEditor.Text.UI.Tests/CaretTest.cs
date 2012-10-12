using System;
using CodeEditor.Text.UI.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Tests
{
	[TestFixture]
	public class CaretTest
	{
		Caret _caret;

		[SetUp]
		public void Setup()
		{
			var caretBoundsMock = new Mock<ICaretBounds>();
			caretBoundsMock.SetupGet(o => o.Rows).Returns(3);
			caretBoundsMock.Setup(o => o.ColumnsForRow(0)).Returns(2);
			caretBoundsMock.Setup(o => o.ColumnsForRow(1)).Returns(3);
			caretBoundsMock.Setup(o => o.ColumnsForRow(2)).Returns(2);
			_caret = new Caret(caretBoundsMock.Object);
		}

		[TestCaseAttribute(0, 0)]
		[TestCaseAttribute(2, 2)]
		public void SetPosition(int row, int column)
		{
			GenericCaretTest(row, column, () => { }, row, column);
		}

		[TestCaseAttribute(-1, 0)]
		[TestCaseAttribute(3, 0)]
		[TestCaseAttribute(0, -1)]
		[TestCaseAttribute(0, 3)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SetPositionThrowsForInvalidArgument(int row, int column)
		{
			GenericCaretTest(row, column, () => { }, 0, 0);
		}

		[Test]
		public void MoveLeftWhenColumnIsNotZero()
		{
			GenericCaretTest(0, 1, () => _caret.MoveLeft(), 0, 0);
		}

		[Test]
		public void MoveLeftWhenColumnIsZeroAndRowIsNotZero()
		{
			GenericCaretTest(1, 0, () => _caret.MoveLeft(), 0, 2);
		}

		[Test]
		public void MoveLeftWhenColumnAndRowAreZero()
		{
			GenericCaretTest(0, 0, () => _caret.MoveLeft(), 0, 0);
		}

		[Test]
		public void MoveRightWhenColumnIsNotLast()
		{
			GenericCaretTest(0, 1, () => _caret.MoveRight(), 0, 2);
		}

		[Test]
		public void MoveRightWhenColumnIsLastAndRowIsNotLast()
		{
			GenericCaretTest(1, 3, () => _caret.MoveRight(), 2, 0);
		}

		[Test]
		public void MoveRightWhenColumnAndRowAreLast()
		{
			GenericCaretTest(2, 2, () => _caret.MoveRight(), 2, 2);
		}

		[Test]
		public void MoveUpWhenAmountIsCurrentRow()
		{
			GenericCaretTest(1, 0, () => _caret.MoveUp(1), 0, 0);
		}

		[Test]
		public void MoveUpWhenAmountIsSmallerThanCurrentRow()
		{
			GenericCaretTest(2, 0, () => _caret.MoveUp(1), 1, 0);
		}

		[Test]
		public void MoveUpWhenAmountIsLargerThanCurrentRow()
		{
			GenericCaretTest(1, 0, () => _caret.MoveUp(2), 0, 0);
		}

		[Test]
		public void MoveUpWhenColumnIsLargerThanColumnsForTargetRow()
		{
			GenericCaretTest(1, 3, () => _caret.MoveUp(1), 0, 2);
		}

		[Test]
		public void MoveDownWhenAmountIsRowsBelowCurrent()
		{
			GenericCaretTest(0, 0, () => _caret.MoveDown(2), 2, 0);
		}

		[Test]
		public void MoveDownWhenAmountIsSmallerThanRowsBelowCurrent()
		{
			GenericCaretTest(0, 0, () => _caret.MoveDown(1), 1, 0);
		}

		[Test]
		public void MoveDownWhenAmountIsLargerThanRowsBelowCurrent()
		{
			GenericCaretTest(0, 0, () => _caret.MoveDown(3), 2, 0);
		}

		[Test]
		public void MoveDownWhenColumnIsLargerThanColumnsForTargetRow()
		{
			GenericCaretTest(1, 3, () => _caret.MoveDown(1), 2, 2);
		}

		[Test]
		public void MoveToStart()
		{
			GenericCaretTest(1, 1, () => _caret.MoveToStart(), 0, 0);
		}

		[Test]
		public void MoveToEnd()
		{
			GenericCaretTest(0, 0, () => _caret.MoveToEnd(), 2, 2);
		}

		[Test]
		public void MoveToRowStart()
		{
			GenericCaretTest(1, 1, () => _caret.MoveToRowStart(), 1, 0);
		}

		[Test]
		public void MoveToRowEnd()
		{
			GenericCaretTest(1, 0, () => _caret.MoveToRowEnd(), 1, 3);
		}

		void GenericCaretTest(int intialRow, int initialColumn, Action action, int expectedRow, int expectedColumn)
		{
			_caret.SetPosition(intialRow, initialColumn);
			action();
			Assert.AreEqual(expectedRow, _caret.Row);
			Assert.AreEqual(expectedColumn, _caret.Column);
		}
	}
}
