using Moq;
using NUnit.Framework;

namespace CodeEditor.Testing
{
	public abstract class MockBasedTest
	{
		[SetUp]
		public void SetUpMockRepository()
		{
			_mocks = new MockRepository(MockBehavior.Strict);
		}

		protected Mock<T> MockFor<T>() where T : class
		{
			return MockFor<T>(MockBehavior.Default);
		}

		protected Mock<T> MockFor<T>(MockBehavior mockBehavior) where T : class
		{
			return _mocks.Create<T>(mockBehavior);
		}

		protected void VerifyAllMocks()
		{
			_mocks.VerifyAll();
		}

		private MockRepository _mocks;
	}
}
