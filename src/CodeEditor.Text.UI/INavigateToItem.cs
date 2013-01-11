namespace CodeEditor.Text.UI
{
	public interface INavigateToItem
	{
		string DisplayText { get; set; }
		void NavigateTo();
	}
}
