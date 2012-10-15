using CodeEditor.IO;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewDocumentFactory
	{
		ITextViewDocument DocumentForFile(IFile file);
	}
}
