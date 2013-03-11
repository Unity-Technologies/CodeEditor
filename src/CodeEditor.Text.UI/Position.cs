namespace CodeEditor.Text.UI
{
	public struct Position
	{
		public Position(int row, int column) : this()
		{
			Column = column;
			Row = row;
		}

		public int Column { get; set; }

		public int Row { get; set; }

		public static bool operator ==(Position lhs, Position rhs)
		{
			return lhs.Row == rhs.Row && lhs.Column == rhs.Column;
		}

		public static bool operator !=(Position lhs, Position rhs)
		{
			return lhs.Column != rhs.Column || lhs.Row != rhs.Row;
		}

		public override string ToString()
		{
			return string.Format("(row {0}, column {1})", Row, Column);
		}
	}
}