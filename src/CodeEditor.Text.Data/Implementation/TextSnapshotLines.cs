using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Collections;

namespace CodeEditor.Text.Data.Implementation
{
	public class TextSnapshotLines : ITextSnapshotLines
	{
		private readonly PieceTable<LineSpan> _spans;
		private Immutable<TextSnapshot> _owner;

		public TextSnapshotLines(PieceTable<char> text)
		{
			_spans = PieceTable.ForArray(LineParser.LineSpansFor(text).ToArray());
		}

		internal TextSnapshot Owner
		{
			get { return _owner.Value; }
			set { _owner.Value = value; }
		}

		public int Count
		{
			get { return _spans.Length; }
		}

		public ITextSnapshotLine this[int lineNumber]
		{
			get
			{
				if (lineNumber < 0 || lineNumber >= _spans.Length)
					throw new ArgumentOutOfRangeException("lineNumber", lineNumber, string.Format("{0} is not a valid line number.", lineNumber));
				var lineSpan = _spans[lineNumber].GetValue();
				return new TextSnapshotLine(Owner, lineSpan, lineNumber);
			}
		}

		public TextSnapshotLines Insert(int position, string text)
		{
			throw new NotImplementedException();
		}

		public TextSnapshotLines Delete(int position, int amount)
		{
			throw new NotImplementedException();
		}

		public int LineNumberForPosition(int position)
		{
			var max = LastLineIndex;
			var min = 0;
			while (max >= min)
			{
				var pivot = (min + max) / 2;
				var partition = _spans[pivot].GetValue();
				if (position >= partition.Start)
				{
					if (position < partition.End)
						return pivot;
					min = pivot + 1;
					continue;
				}
				max = pivot - 1;
			}
			return LastLineSpan.IsTerminated ? Count : Count - 1;
		}

		private int LastLineIndex
		{
			get { return Count - 1; }
		}

		LineSpan LastLineSpan
		{
			get { return _spans[LastLineIndex].GetValue(); }
		}

		public IEnumerator<ITextSnapshotLine> GetEnumerator()
		{
			return Enumerable.Range(0, Count).Select(line => this[line]).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}