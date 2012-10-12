using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI
{
	public interface ICaretFactory
	{
		ICaret CaretForBuffer(ITextBuffer buffer);
	}
}
