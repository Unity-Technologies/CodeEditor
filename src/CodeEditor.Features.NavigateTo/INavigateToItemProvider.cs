using CodeEditor.Reactive;

namespace CodeEditor.Features.NavigateTo
{
	public interface INavigateToItemProvider
	{
		IObservableX<INavigateToItem> Search(string filter);
	}

	public interface INavigateToItem
	{
		string DisplayText { get; }
		void NavigateTo();
	}
}
