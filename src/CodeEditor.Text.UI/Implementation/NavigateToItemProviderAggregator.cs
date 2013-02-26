using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Reactive;

namespace CodeEditor.Text.UI.Implementation
{
	[Export(typeof(INavigateToItemProviderAggregator))]
	public class NavigateToItemProviderAggregator : INavigateToItemProviderAggregator
	{
		[ImportMany]
		public INavigateToItemProvider[] Providers { get; set; }

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return Providers.Select(provider => provider.Search(filter)).Merge();
		}
	}
}
