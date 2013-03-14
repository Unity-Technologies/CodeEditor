
namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewTabs
	{
		int NumberOfWhitespacesPerTab { get; set; }
		string ReplaceTabsWithWhiteSpaces(string text, bool showWhitespaces);
	}
}
