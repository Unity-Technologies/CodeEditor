using NUnit.Framework;

namespace CodeEditor.Composition.Tests
{
	[TestFixture]
	public class LazyTest
	{
		[Test]
		public void FactoryIsInvokedOnlyOnce()
		{
			var count = 0;
			var lazy = new Lazy<object>(() => (object) ++count);
			Assert.AreEqual(0, count);

			Assert.AreEqual(1, lazy.Value);
			Assert.AreEqual(1, count);

			Assert.AreEqual(1, lazy.Value);
			Assert.AreEqual(1, count);
		}
	}
}
