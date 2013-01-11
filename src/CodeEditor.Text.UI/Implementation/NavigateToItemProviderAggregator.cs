using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Implementation
{
	[Export(typeof(INavigateToItemProviderAggregator))]
	public class NavigateToItemProviderAggregator : INavigateToItemProviderAggregator
	{
		[ImportMany]
		public INavigateToItemProvider[] Providers { get; set; }

		public List<INavigateToItem> Search(string filter)
		{
			return Providers.SelectMany(provider => provider.Search(filter)).ToList();
		}
	}
}
