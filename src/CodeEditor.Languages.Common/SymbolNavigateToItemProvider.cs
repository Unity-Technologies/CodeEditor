using CodeEditor.Composition;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Server.Interface;
using CodeEditor.Text.UI;

namespace CodeEditor.Languages.Common
{
	[Export(typeof(INavigateToItemProvider))]
	public class SymbolNavigateToItemProvider : INavigateToItemProvider
	{
		[Import]
		public IObservableServiceClientProvider ServiceClientProvider { get; set; }

		[Import]
		public ILogger Logger { get; set; }

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return string.IsNullOrEmpty(filter)
				? ObservableX.Empty<INavigateToItem>()
				: ServiceClient
					.SelectMany(
						(client) => client.ObserveMany(new SymbolSearch {Filter = filter}),
						(client, symbol) => (INavigateToItem) new SymbolItem(symbol));
		}

		private IObservableX<IObservableServiceClient> ServiceClient
		{
			get { return ServiceClientProvider.Client; }
		}

		internal class SymbolItem : INavigateToItem
		{
			private readonly Symbol _symbol;

			public SymbolItem(Symbol symbol)
			{
				_symbol = symbol;
			}

			public string DisplayText
			{
				get { return _symbol.DisplayText; }
			}

			public void NavigateTo()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}