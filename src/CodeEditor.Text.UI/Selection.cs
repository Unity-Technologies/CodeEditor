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
				return new Position (
					Anchor.Row < Caret.Row ? Anchor.Row : Caret.Row,
					(Anchor.Row < Caret.Row || Anchor.Column < Caret.Column) ? Anchor.Column : Caret.Column
					);
			}
		}

		public Position EndDrawPos
		{
			get
			{
				return new Position(
					Caret.Row > Anchor.Row ? Caret.Row : Anchor.Row,
					(Caret.Row > Anchor.Row  || Anchor.Column < Caret.Column) ? Caret.Column : Anchor.Column
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
			return string.Format ("{0} -> {1}", BeginDrawPos, EndDrawPos);
		}

	}
}
