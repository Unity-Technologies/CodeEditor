namespace CodeEditor.Text.UI.Unity.Engine
{
	interface ITextViewWhitespaceProvider
	{
		ITextViewWhitespace GetWhitespace(ISettings settings);
	}
}
