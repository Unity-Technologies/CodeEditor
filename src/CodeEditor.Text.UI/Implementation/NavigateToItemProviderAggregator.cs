using System;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Logging;
using CodeEditor.Reactive;

namespace CodeEditor.Text.UI.Implementation
{
	[Export(typeof(INavigateToItemProviderAggregator))]
	public class NavigateToItemProviderAggregator : INavigateToItemProviderAggregator
	{
		[ImportMany]
		public INavigateToItemProvider[] Providers { get; set; }

		[Import]
		public ILogger Logger { get; set; }

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return Providers
				.Select(provider =>
					provider
					.Search(filter)
					.Catch((Exception exception) =>
					{
						Logger.LogError(exception);
						return ObservableX.Empty<INavigateToItem>();
					}))
				.Merge();
		}
	}
}
