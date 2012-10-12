using CodeEditor.IO;

namespace CodeEditor.Text.Data
{
	public interface ITextDocumentFactory
	{
		ITextDocument ForFile(IFile file);
	}

	public interface ITextDocument
	{
		IFile File { get; }
		ITextBuffer Buffer { get; }
		void Save();
	}
}
