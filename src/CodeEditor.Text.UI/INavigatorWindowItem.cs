namespace CodeEditor.Text.UI
{
	public interface INavigatorWindowItem
	{
		string DisplayText { get; set; }
		void NavigateTo();
	}
}
