using System;
using CodeEditor.Collections;

namespace CodeEditor.Text.Data.Implementation
{
	class TextSnapshot : ITextSnapshot
	{
		private readonly ITextBuffer _owner;
		private readonly PieceTable<char> _text;
		private readonly TextSnapshotLines _lines;

		public TextSnapshot(ITextBuffer owner, PieceTable<char> text) : this(owner, text, new TextSnapshotLines(text))
		{
		}

		public TextSnapshot(ITextBuffer owner, PieceTable<char> text, TextSnapshotLines lines)
		{
			_owner = owner;
			_text = text;
			_lines = lines;
			_lines.Owner = this;
		}

		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
					new ArgumentOutOfRangeException("Index out of range: " + index);
				return _text[index].GetValue();
			}
		}

		public ITextBuffer Buffer
		{
			get { return _owner; }
		}

		public string Text
		{
			get { return GetText(0, Length); }
		}

		public int Length
		{
			get { return _text.Length; }
		}

		public ITextSnapshotLines Lines
		{
			get { return _lines; }
		}

		public string GetText(Span span)
		{
			return GetText(span.Start, span.Length);
		}

		public TextSpan GetSpan(int position, int length)
		{
			return new TextSpan(this, new Span(position, length));
		}

		public int LineNumberForPosition(int position)
		{
			return _lines.LineNumberForPosition(position);
		}

		public string GetText(int index, int amount)
		{
			if (index < 0 || index + amount > Length)
				new ArgumentOutOfRangeException("Index out of range: " + index);

			if (amount == 0)
				return string.Empty;

			return new string(_text.ToArray(index, amount));
		}

		public TextSnapshot Delete(int index, int amount)
		{
			var newText = _text.Delete(index, amount);
			return new TextSnapshot(_owner, newText, new TextSnapshotLines(newText));
			//return new TextSnapshot(_owner, newText, _lines.Delete(index, amount));
		}

		public TextSnapshot Insert(int index, string text)
		{
			// TODO: consider inserting large strings as a single piece
			var newText = _text;
			for (var i = 0; i < text.Length; ++i)
				newText = newText.Insert(index++, text[i]);

			return new TextSnapshot(_owner, newText, new TextSnapshotLines(newText));
			//return new TextSnapshot(_owner, newText, _lines.Insert(index, text));
		}
	}
}
