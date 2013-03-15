
namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewWhitespace
	{
		int NumberOfSpacesPerTab { get; set; }
		bool Visible { get; set; }
		string ReplaceWhitespace(string text);
	}
}
