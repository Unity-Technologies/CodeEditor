using CodeEditor.Composition;
using CodeEditor.Reactive;
using CodeEditor.Server.Interface;
using CodeEditor.Text.UI;

namespace CodeEditor.Languages.Common
{
	[Export(typeof(INavigateToItemProvider))]
	internal class SymbolNavigateToItemProvider : INavigateToItemProvider
	{
		[Import]
		public Lazy<IUnityProjectProvider> UnityProjectProvider;

		public IObservableX<INavigateToItem> Search(string filter)
		{
			return Project.SearchSymbol(filter).Select(symbol => new SymbolItem(symbol));
		}

		private IUnityProject Project
		{
			get { return UnityProjectProvider.Value.Project; }
		}

		internal class SymbolItem : INavigateToItem
		{
			private readonly ISymbol _symbol;

			public SymbolItem(ISymbol symbol)
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