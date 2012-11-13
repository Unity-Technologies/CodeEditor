using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewSelection
	{
		public TextViewPosition Anchor {get; set;} 
		readonly ITextViewDocument _document;

		public TextViewSelection (ITextViewDocument document)
		{
			_document = document;
			Clear ();
		}

		public TextViewPosition BeginDrawPos
		{
			get 
			{
				return new TextViewPosition (
					(Anchor.row < Caret.Row || Anchor.column < Caret.Column) ? Anchor.column : Caret.Column,
					Anchor.row < Caret.Row ? Anchor.row : Caret.Row);
			}
		}

		public TextViewPosition EndDrawPos
		{
			get
			{
				return new TextViewPosition(
					(Caret.Row > Anchor.row  || Anchor.column < Caret.Column) ? Caret.Column : Anchor.column,
					Caret.Row > Anchor.row ? Caret.Row : Anchor.row);
			}
		}

		private ICaret Caret
		{
			get { return _document.Caret; }
		}


		public void Clear ()
		{
			Anchor = new TextViewPosition(-1,-1);
		}

		public bool HasSelection ()
		{
			return Anchor.row >= 0 && !(Anchor.row == Caret.Row && Anchor.column == Caret.Column);
		}

		public override string ToString ()
		{
			return string.Format ("{0} -> {1}", BeginDrawPos, EndDrawPos);
		}

	}
}
