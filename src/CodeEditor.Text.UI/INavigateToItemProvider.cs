using CodeEditor.Reactive;

namespace CodeEditor.Text.UI
{
	public interface INavigateToItemProvider
	{
		IObservableX<INavigateToItem> Search(string filter);
	}
}
