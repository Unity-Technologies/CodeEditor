using System.Linq;
using CodeEditor.Text.Data;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.Logic.Tests
{
	[TestFixture]
	public class ClassificationSpansTests
	{
		[Test]
		public void MergingTwoDifferentReturnsBoth()
		{
			var classificationSpans = new[]
			{
				new ClassificationSpan(new Mock<IClassification>().Object, new TextSpan(null, new Span(0, 1))),
				new ClassificationSpan(new Mock<IClassification>().Object, new TextSpan(null, new Span(1, 1)))
			};
			Assert.AreEqual(classificationSpans, ClassificationSpans.Merge(classificationSpans));
		}

		[Test]
		public void MergingTwoEqualReturnsMerged()
		{
			var classification = new Mock<IClassification>().Object;
			var classificationSpans = new[]
			{
				new ClassificationSpan(classification, new TextSpan(null, new Span(0, 1))),
				new ClassificationSpan(classification, new TextSpan(null, new Span(1, 1)))
			};
			CollectionAssert.AreEqual(
				new[] {new ClassificationSpan(classification, new TextSpan(null, new Span(0, 2)))},
				ClassificationSpans.Merge(classificationSpans));
		}

		[Test]
		public void MergingTwoEqualFollowingDifferentReturnsDifferentFollowedByMerged()
		{
			var different = new ClassificationSpan(new Mock<IClassification>().Object, new TextSpan(null, new Span(0, 1)));
			var classification = new Mock<IClassification>().Object;
			var classificationSpans = new[]
			{
				different,
				new ClassificationSpan(classification, new TextSpan(null, new Span(1, 1))),
				new ClassificationSpan(classification, new TextSpan(null, new Span(2, 1)))
			};
			CollectionAssert.AreEqual(
				new[] {different, new ClassificationSpan(classification, new TextSpan(null, new Span(1, 2)))},
				ClassificationSpans.Merge(classificationSpans).ToArray());
		}
	}
}