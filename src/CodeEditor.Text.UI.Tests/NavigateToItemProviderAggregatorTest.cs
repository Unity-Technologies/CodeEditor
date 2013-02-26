using CodeEditor.Reactive;
using CodeEditor.Testing;
using CodeEditor.Text.UI.Implementation;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Tests
{
	[TestFixture]
	public class NavigateToItemProviderAggregatorTest : MockBasedTest
	{
		[Test]
		public void SupportsManyProviders ()
		{
			var providerA = MockFor<INavigateToItemProvider>();
			var itemFromProviderA = MockFor<INavigateToItem>();
			providerA.Setup(_ => _.Search("")).Returns(ObservableX.Return(itemFromProviderA.Object));

			var providerB = MockFor<INavigateToItemProvider>();
			var itemFromProviderB = MockFor<INavigateToItem>();
			providerB.Setup(_ => _.Search("")).Returns(ObservableX.Return(itemFromProviderB.Object));

			var aggregator = new NavigateToItemProviderAggregator { Providers = new[] { providerA.Object, providerB.Object } };
			var aggregatorItems = aggregator.Search("").ToList().FirstOrDefault();
			CollectionAssert.AreEquivalent(new[] { itemFromProviderA.Object, itemFromProviderB.Object}, aggregatorItems);
		}
	}
}
