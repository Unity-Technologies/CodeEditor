using CodeEditor.Composition;
using CodeEditor.Features.NavigateTo.SourceSymbols.ServiceInterface;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.ReactiveServiceStack;
using CodeEditor.ServiceClient;
using CodeEditor.Text.UI;

namespace CodeEditor.Features.NavigateTo.SourceSymbols
{
	[Export(typeof(INavigateToItemProvider))]
	public class SymbolNavigateToItemProvider : INavigateToItemProvider
	{
		[Import]
		public IObservableServiceClientProvider ServiceClientProvider { get; set; }

		[Import]
		public IFileNavigationService FileNavigationService { get; set; }

		[Import]
		public ILogger Logger { get; set; }

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return string.IsNullOrEmpty(filter)
				? ObservableX.Empty<INavigateToItem>()
				: ServiceClient
					.SelectMany(
						(client) => client.ObserveMany(new SourceSymbolSearchRequest {Filter = filter}),
						(client, symbol) => (INavigateToItem) new SymbolItem(symbol, FileNavigationService));
		}

		IObservableX<IObservableServiceClient> ServiceClient
		{
			get { return ServiceClientProvider.Client; }
		}

		internal class SymbolItem : INavigateToItem
		{
			readonly SourceSymbol _symbol;
			readonly IFileNavigationService _fileNavigationService;

			public SymbolItem(SourceSymbol symbol, IFileNavigationService fileNavigationService)
			{
				_symbol = symbol;
				_fileNavigationService = fileNavigationService;
			}

			public string DisplayText
			{
				get { return _symbol.DisplayText; }
			}

			public void NavigateTo()
			{
				var file = _symbol.SourceFile;
				var line = _symbol.Line;
				var column = _symbol.Column;
				_fileNavigationService.NavigateTo(file, new PositionAnchor(line, column));
			}
		}
	}
}