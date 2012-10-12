namespace CodeEditor.Text.UI
{
	public interface ICaretBounds
	{
		int Rows { get; }
		int ColumnsForRow(int row);
	}
}
