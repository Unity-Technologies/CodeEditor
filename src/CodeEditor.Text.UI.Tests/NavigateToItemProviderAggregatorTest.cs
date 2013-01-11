using System.Collections.Generic;
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
			providerA.Setup(_ => _.Search("")).Returns(new List<INavigateToItem> { itemFromProviderA.Object });

			var providerB = MockFor<INavigateToItemProvider>();
			var itemFromProviderB = MockFor<INavigateToItem>();
			providerB.Setup(_ => _.Search("")).Returns(new List<INavigateToItem> { itemFromProviderB.Object });

			var aggregator = new NavigateToItemProviderAggregator { Providers = new[] { providerA.Object, providerB.Object } };
			var aggregatorItems = aggregator.Search("");
			CollectionAssert.AreEquivalent(new[] { itemFromProviderA.Object, itemFromProviderB.Object}, aggregatorItems);
		}
	}
}
