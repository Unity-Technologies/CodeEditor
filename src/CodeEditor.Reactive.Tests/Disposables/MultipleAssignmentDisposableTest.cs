using CodeEditor.Reactive.Disposables;
using NUnit.Framework;

namespace CodeEditor.Reactive.Tests.Disposables
{
	[TestFixture]
	public class MultipleAssignmentDisposableTest
	{
		[Test]
		public void DisposesUnderlyingDisposableOnce()
		{
			var disposed = 0;
			var subject = new MultipleAssignmentDisposable {Disposable = Disposable.Create(() => ++disposed)};
			Assert.AreEqual(0, disposed);
			for (var i = 0; i < 2; ++i)
			{
				subject.Dispose();
				Assert.AreEqual(1, disposed);
			}
		}

		[Test]
		public void ReplacingDisposableDoesNotCauseItToBeDisposed()
		{
			var first = 0;
			var subject = new MultipleAssignmentDisposable {Disposable = Disposable.Create(() => ++first)};

			var second = 0;
			subject.Disposable = Disposable.Create(() => ++second);
			Assert.AreEqual(0, first);
			Assert.AreEqual(0, second);

			subject.Dispose();
			Assert.AreEqual(0, first);
			Assert.AreEqual(1, second);
		}

		[Test]
		public void SettingDisposableAfterDisposeCausesItToBeDisposed()
		{
			var subject = new MultipleAssignmentDisposable();
			subject.Dispose();

			var disposed = false;
			subject.Disposable = Disposable.Create(() => disposed = true);
			Assert.IsTrue(disposed);
		}
	}
}
