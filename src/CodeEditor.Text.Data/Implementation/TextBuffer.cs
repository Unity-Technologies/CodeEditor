using System;
using CodeEditor.Collections;
using CodeEditor.ContentTypes;

namespace CodeEditor.Text.Data.Implementation
{
	public class TextBuffer : ITextBuffer
	{
		private TextSnapshot _currentSnapshot;

		public TextBuffer(string text, IContentType contentType)
			: this(Piece.ForString(NormalizeLineTerminators(text)), contentType)
		{
		}

		private TextBuffer(IPiece<char> text, IContentType contentType)
		{
			ContentType = contentType;
			_currentSnapshot = new TextSnapshot(this, PieceTable.ForPiece(text));
		}

		public event TextChange Changed;

		public IContentType ContentType { get; private set; }

		public ITextSnapshot CurrentSnapshot
		{
			get { return _currentSnapshot; }
		}

		public void Insert(int index, string item)
		{
			if (index < 0 || index > CurrentSnapshot.Length)
				throw new ArgumentOutOfRangeException("Index out of range: " + index);

			if (item == null)
				throw new ArgumentNullException("item");

			if (item == "")
				return;

			item = NormalizeLineTerminators(item);
			OnChanged(_currentSnapshot.Insert(index, item), new Span(index, item.Length), new Span(index, 0));
		}

		public void Delete(int index, int amount)
		{
			if (amount == 0)
				return;

			OnChanged(_currentSnapshot.Delete(index, amount), new Span(index, 0), new Span(index, amount));
		}

		private void OnChanged(TextSnapshot newSnapshot, Span newSpan, Span oldSpan)
		{
			var oldSnapshot = _currentSnapshot;
			_currentSnapshot = newSnapshot;
			if (Changed != null)
				Changed(this, new TextChangeArgs(oldSnapshot, oldSpan, newSnapshot, newSpan));
		}

		private static string NormalizeLineTerminators(string text)
		{
			return text.Replace("\r\n", "\n");
		}
	}
}
