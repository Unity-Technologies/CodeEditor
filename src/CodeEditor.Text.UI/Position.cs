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
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Position lhs, Position rhs)
		{
			return !lhs.Equals(rhs);
		}

		public bool Equals(Position other)
		{
			return Column == other.Column && Row == other.Row;
		}

		public override string ToString()
		{
			return string.Format("(row {0}, column {1})", Row, Column);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Position && Equals((Position)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Column * 397) ^ Row;
			}
		}
	}
}