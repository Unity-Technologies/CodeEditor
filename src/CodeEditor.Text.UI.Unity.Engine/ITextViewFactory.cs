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
		/// Creates a view for the specified file taken from the imported <see cref="IFileSystem"/>
		/// with margins provided by the imported <see cref="IDefaultTextViewMarginsProvider" />.
		/// </summary>
		ITextView ViewForFile(string fileName);

		/// <summary>
		/// Creates a view with the specified options.
		/// </summary>
		ITextView CreateView(TextViewCreationOptions options);
	}

	public class TextViewCreationOptions
	{
		/// <summary>
		/// File to be viewed. Defaults to a transient txt file.
		/// </summary>
		public IFile File { get; set; }

		/// <summary>
		/// Margins to be included. Defaults to <see cref="IDefaultTextViewMarginsProvider.MarginsFor"/>.
		/// </summary>
		public ITextViewMargins Margins { get; set; }
	}
}
