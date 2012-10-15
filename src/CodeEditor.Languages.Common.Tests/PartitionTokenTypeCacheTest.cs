using Boo.Ide.Grammars;
using CodeEditor.Testing;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Implementation;
using NUnit.Framework;

namespace CodeEditor.Languages.Common.Tests
{
	public static class PartitionTokenTypeCacheTest
	{
		[TestFixture]
		public class LastPartitionTokenTypeBefore : MockBasedTest
		{
			[Test]
			public void WillNotCallTheTokenizerForTheFirstLine()
			{
				var firstLine = MockFor<ITextSnapshotLine>();
				firstLine
					.SetupGet(_ => _.LineNumber)
					.Returns(0);

				var subject = new PartitionTokenTypeCache(MockFor<IPartitionTokenizer>().Object);
				Assert.AreEqual(PartitionTokenType.None, subject.LastPartitionTokenTypeBefore(firstLine.Object));

				VerifyAllMocks();
			}

			[Test]
			public void WillNotCallTheTokenizerTwiceForTheSameLine()
			{
				var firstLineText = " ";

				var tokenizer = MockFor<IPartitionTokenizer>();
				tokenizer
					.Setup(_ => _.Tokenize(PartitionTokenType.None, firstLineText))
					.Returns(new[] { new PartitionToken(0, firstLineText.Length, PartitionTokenType.Code)});

				var buffer = new TextBuffer(firstLineText + "\n", ContentType());
				var secondLine = buffer.CurrentSnapshot.Lines[1];

				var subject = new PartitionTokenTypeCache(tokenizer.Object);
				Assert.AreEqual(PartitionTokenType.Code, subject.LastPartitionTokenTypeBefore(secondLine));
				Assert.AreEqual(PartitionTokenType.Code, subject.LastPartitionTokenTypeBefore(secondLine));

				VerifyAllMocks();
			}

			private IContentType ContentType()
			{
				return MockFor<IContentType>().Object;
			}
		}
	}
}
