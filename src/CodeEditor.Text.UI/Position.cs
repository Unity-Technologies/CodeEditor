using System;

namespace CodeEditor.Text.UI
{
	public struct Position 
	{
		int _column, _row;
		public Position (int row, int column)
		{
			_column = column;
			_row = row;
		}

		public int Column {get{return _column;} set{_column = value;}}
		public int Row {get{return _row;}  set{_row=value;}}

		public override string ToString ()
		{
			return string.Format ("(row {0}, column {1})", Row, Column);
		}
	}
}
