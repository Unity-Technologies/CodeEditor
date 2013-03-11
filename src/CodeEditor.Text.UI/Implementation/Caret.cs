using System;

namespace CodeEditor.Text.UI.Implementation
{
	public class Caret : ICaret
	{
		readonly ICaretBounds _caretBounds;

		public Caret(ICaretBounds caretBounds)
		{
			_caretBounds = caretBounds;
		}

		public int Row { get; private set; }
		public int Column { get; private set; }

		public void SetPosition(int row, int column)
		{
			if (row < 0 || row > LastRowIndex)
				throw new ArgumentOutOfRangeException("row", string.Format("row must be a value between 0 and {0}, was {1}", LastRowIndex, row));
			if (column < 0 || column > ColumnsForRow(row))
				throw new ArgumentOutOfRangeException("column", string.Format("column must be a value between 0 and {0}, was {1}", ColumnsForRow(row), column));

			Row = row;
			Column = column;
			OnMoved();
		}

		private void OnMoved()
		{
			var handler = Moved;
			if (handler != null)
				handler();
		}

		public void MoveLeft()
		{
			if (Column > 0)
				--Column;
			else if (Row > 0)
			{
				--Row;
				Column = ColumnsForRow(Row);
			}
			OnMoved();
		}

		public void MoveRight()
		{
			if (Column < ColumnsForRow(Row))
				++Column;
			else if (Row < LastRowIndex)
			{
				++Row;
				Column = 0;
			}
			OnMoved();
		}

		public void MoveUp(int rows)
		{
			MoveToRow(Row - rows);
		}

		public void MoveDown(int rows)
		{
			MoveToRow(Row + rows);
		}

		public void MoveToRowStart()
		{
			Column = 0;
			OnMoved();
		}

		public void MoveToRowEnd()
		{
			Column = ColumnsForRow(Row);
			OnMoved();
		}

		public void MoveToStart()
		{
			Row = 0;
			Column = 0;
			OnMoved();
		}

		public void MoveToEnd()
		{
			Row = LastRowIndex;
			Column = ColumnsForRow(LastRowIndex);
			OnMoved();
		}

		public event Action Moved;

		void MoveToRow(int row)
		{
			Row = Math.Max(0, Math.Min(LastRowIndex, row));
			Column = Math.Min(ColumnsForRow(Row), Column);
			OnMoved();
		}

		int LastRowIndex
		{
			get { return _caretBounds.Rows - 1; }
		}

		int ColumnsForRow(int row)
		{
			return _caretBounds.ColumnsForRow(row);
		}
	}
}
