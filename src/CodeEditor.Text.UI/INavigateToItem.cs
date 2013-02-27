namespace CodeEditor.Text.UI
{
	public interface INavigateToItem
	{
		string DisplayText { get; }
		void NavigateTo();
	}
}
