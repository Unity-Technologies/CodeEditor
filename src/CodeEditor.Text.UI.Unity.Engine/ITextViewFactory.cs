using CodeEditor.IO;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewFactory
	{
		/// <summary>
		/// Creates a view with an empty document of content type "text".
		/// </summary>
		ITextView CreateView();

		/// <summary>
		/// Creates a view for the specific file taken from the current <see cref="IFileSystem"/>.
		/// </summary>
		ITextView ViewForFile(string fileName);
	}
}
