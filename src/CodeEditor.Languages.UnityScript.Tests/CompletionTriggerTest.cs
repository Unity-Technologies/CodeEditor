using CodeEditor.Languages.Common;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Tests;
using CodeEditor.Text.UI.Completion;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Languages.Tests
{
	[TestFixture]
	public class CompletionTriggerTest : TextBufferBasedTest
	{
		[Test]
		public void CompletionSessionIsStartedWhenProviderReturnsCompletions()
		{
			var completions = NonEmptyCompletionSet();

			var provider = MockFor<ICompletionProvider>();
			provider
				.Setup(p => p.CompletionsFor(It.IsAny<TextSpan>()))
				.Returns(completions);

			var completionSessionProvider = MockFor<ICompletionSessionProvider>();
			completionSessionProvider.Setup(csp => csp.StartCompletionSession(It.IsAny<TextSpan>(), completions));

			new CompletionTrigger(Buffer, provider.Object, completionSessionProvider.Object);
			Buffer.Insert(0, "t");

			VerifyAllMocks();
		}

		[Test]
		public void CompletionSessionIsNotStartedWhenThereAreNoCompletions()
		{
			var provider = MockFor<ICompletionProvider>();
			provider
				.Setup(p => p.CompletionsFor(It.IsAny<TextSpan>()))
				.Returns(EmptyCompletionSet());

			var completionSessionProvider = MockFor<ICompletionSessionProvider>();

			new CompletionTrigger(Buffer, provider.Object, completionSessionProvider.Object);
			Buffer.Insert(0, "t");

			VerifyAllMocks();
		}

		private ICompletionSet EmptyCompletionSet()
		{
			return CompletionSetWithIsEmptyEqualsTo(true);
		}

		private ICompletionSet NonEmptyCompletionSet()
		{
			return CompletionSetWithIsEmptyEqualsTo(false);
		}

		private ICompletionSet CompletionSetWithIsEmptyEqualsTo(bool isEmpty)
		{
			var completions = MockFor<ICompletionSet>();
			completions.SetupGet(c => c.IsEmpty).Returns(isEmpty);
			return completions.Object;
		}

		[SetUp]
		public void SetUp()
		{
			_mocks = new MockRepository(MockBehavior.Strict);
		}

		private Mock<T> MockFor<T>() where T : class
		{
			return _mocks.Create<T>();
		}

		private void VerifyAllMocks()
		{
			_mocks.VerifyAll();
		}

		private MockRepository _mocks;
	}
}
