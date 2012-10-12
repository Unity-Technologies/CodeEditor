using System;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Tests;
using CodeEditor.Text.UI.Implementation;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Tests
{
	[TestFixture]
	public class DefaultTextStructureNavigatorTest : TextBufferBasedTest
	{
		[SetUp]
		public void SetUp()
		{
			SetText(" a_1 \t*.^&\n\n");
		}

		[TestCase(-1)]
		[TestCase(13)]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void GetSpanForThrowsWhenPositionIsOutOfBounds(int position)
		{
			GenericTest(n => n.GetSpanFor(position, CurrentSnapshot), 0, 0);
		}

		[TestCase(0, 1)]
		[TestCase(1, 3)]
		[TestCase(4, 2)]
		[TestCase(6, 1)]
		[TestCase(8, 1)]
		[TestCase(11, 1)]
		[TestCase(12, 0)]
		public void GetSpan(int position, int expectedLength)
		{
			GenericTest(n => n.GetSpanFor(position, CurrentSnapshot), position, expectedLength);
		}

		[Test]
		public void GetPreviousSpan()
		{
			GenericTest(n => n.GetPreviousSpanFor(TextSpan(7, 1)), 6, 1);
		}

		[Test]
		public void GetNextSpan()
		{
			GenericTest(n => n.GetNextSpanFor(TextSpan(1, 3)), 4, 2);
		}

		[Test]
		public void GetPreviousSpanReturnsSpanOfZeroLengthAtStartOfBuffer()
		{
			GenericTest(n => n.GetPreviousSpanFor(TextSpan(0, 1)), 0, 0);
		}

		[Test]
		public void GetNextSpanReturnsSpanOfZeroLengthAtEndOfBuffer()
		{
			GenericTest(n => n.GetNextSpanFor(TextSpan(11, 1)), 12, 0);
		}

		void GenericTest(Func<ITextStructureNavigator, TextSpan> func, int expectedStart, int expectedLength)
		{
			var nav = new DefaultTextStructureNavigator();
			var result = func(nav);
			Assert.AreEqual(TextSpan(expectedStart, expectedLength), result);
		}

		TextSpan TextSpan(int start, int length)
		{
			return CurrentSnapshot.GetSpan(start, length);
		}
	}
}
