using System;

namespace CodeEditor.Text.UI
{
	public class Selection
	{
		public Position Anchor {get; set;} 
		readonly ICaret _caret;

		public Selection (ICaret caret)
		{
			_caret = caret;
			Clear ();
		}

		public Position BeginDrawPos
		{
			get 
			{
				int column;
				if (Anchor.Row == Caret.Row)
					column = Math.Min(Anchor.Column, Caret.Column);
				else if (Anchor.Row < Caret.Row)
					column = Anchor.Column;
				else
					column = Caret.Column;

				return new Position (
					Anchor.Row < Caret.Row ? Anchor.Row : Caret.Row,
					column
					);
			}
		}

		public Position EndDrawPos
		{
			get
			{
				int column;
				if (Anchor.Row == Caret.Row)
					column = Math.Max(Anchor.Column, Caret.Column);
				else if (Anchor.Row < Caret.Row)
					column = Caret.Column;
				else
					column = Anchor.Column;

				return new Position(
					Caret.Row > Anchor.Row ? Caret.Row : Anchor.Row,
					column
					);
			}
		}

		private ICaret Caret
		{
			get { return _caret; }
		}


		public void Clear ()
		{
			Anchor = new Position(-1,-1);
		}

		public bool HasSelection ()
		{
			return Anchor.Row >= 0 && !(Anchor.Row == Caret.Row && Anchor.Column == Caret.Column);
		}

		public override string ToString ()
		{
			return string.Format ("{0} -> {1}, Anchor: {2}, Caret {3},{4}", BeginDrawPos, EndDrawPos, Anchor, Caret.Row, Caret.Column);
		}

	}
}
