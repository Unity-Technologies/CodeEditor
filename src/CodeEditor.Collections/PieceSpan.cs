namespace CodeEditor.Collections
{
	public static class PieceSpan
	{
		public static PieceSpan<T> For<T>(IPiece<T> piece, int start)
		{
			return new PieceSpan<T>(piece, start);
		}
	}

	public struct PieceSpan<T>
	{
		public readonly IPiece<T> Piece;
		public readonly int Start;

		public PieceSpan(IPiece<T> piece, int start)
		{
			Start = start;
			Piece = piece;
		}

		public int End
		{
			get { return Start + Length; }
		}

		public int Length
		{
			get { return Piece.Length; }
		}
	}
}
