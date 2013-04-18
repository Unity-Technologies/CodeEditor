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
				if (Caret.Column + Caret.Row*1000 > Anchor.Column + Anchor.Row*1000)
					return Anchor;
				
				return new Position(Caret.Row, Caret.Column);
			}
		}

		public Position EndDrawPos
		{
			get
			{
				if (Caret.Column + Caret.Row * 1000 < Anchor.Column + Anchor.Row * 1000)
					return Anchor;

				return new Position(Caret.Row, Caret.Column);
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
