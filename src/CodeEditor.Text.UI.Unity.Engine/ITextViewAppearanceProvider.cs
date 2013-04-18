namespace CodeEditor.Text.UI.Unity.Engine
{
	/// <summary>
	/// Must be implemented by the <see cref="ITextView"/> host.
	/// </summary>
	public interface ITextViewAppearanceProvider
	{
		ITextViewAppearance AppearanceFor(ITextViewDocument document, IFontManager fontManager);
	}
}
