using System;

namespace CodeEditor.Collections
{
	public static class PieceTable
	{
		public static PieceTable<char> ForString(string s)
		{
			return ForPiece(Piece.ForString(s));
		}

		public static PieceTable<T> ForArray<T>(T[] array)
		{
			return ForPiece(Piece.ForArray(array));
		}

		public static PieceTable<T> ForPiece<T>(IPiece<T> piece)
		{
			return new PieceTable<T>(piece);
		}
	}

	/// <summary>
	/// A fully persistent implementation of the piece table data structure
	/// described in "Data Structures for Text Sequences" by Charles Crowley.
	///
	/// The data structure is optimized for the most common operations in
	/// text editing scenarios and makes supporting unlimited undo a breeze.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PieceTable<T> : IPiece<T>
	{
		private const int PartitionThreshold = 64;

		private readonly InsertionBuffer<T> _insertionBuffer;

		private readonly PieceSpan<T>[] _pieceSpans;

		public PieceTable(IPiece<T> piece)
		{
			_pieceSpans = new[] { PieceSpan.For(piece, 0) };
			_insertionBuffer = new InsertionBuffer<T>();
		}

		PieceTable(PieceSpan<T>[] pieceSpans, InsertionBuffer<T> insertionBuffer)
		{
			_pieceSpans = pieceSpans;
			_insertionBuffer = insertionBuffer;
		}

		public Point this[int position]
		{
			get { return new Point(position, _pieceSpans, PieceIndexForPosition(position)); }
		}

		T IPiece<T>.this[int position]
		{
			get { return this[position].GetValue(); }
		}

		public struct Point
		{
			public readonly int Position;

			private readonly PieceSpan<T>[] _pieceSpans;
			private readonly int _pieceIndex;

			public Point(int position, PieceSpan<T>[] pieceSpans, int pieceIndex)
			{
				Position = position;
				_pieceSpans = pieceSpans;
				_pieceIndex = pieceIndex;
			}

			public T GetValue()
			{
				var pieceSpan = PieceSpan;
				var positionInPiece = Position - pieceSpan.Start;
				return pieceSpan.Piece[positionInPiece];
			}

			public Point Next
			{
				get
				{
					var pieceSpan = PieceSpan;
					var newPosition = Position + 1;
					var pieceIndex = newPosition < pieceSpan.End ? _pieceIndex : _pieceIndex + 1;
					return new Point(newPosition, _pieceSpans, pieceIndex);
				}
			}

			public Point Previous
			{
				get
				{
					var newPosition = Position - 1;
					if (_pieceIndex >= _pieceSpans.Length)
						return new Point(newPosition, _pieceSpans, _pieceIndex - 1);

					var pieceSpan = PieceSpan;
					var pieceIndex = newPosition >= pieceSpan.Start ? _pieceIndex : _pieceIndex - 1;
					return new Point(newPosition, _pieceSpans, pieceIndex);
				}
			}

			private PieceSpan<T> PieceSpan
			{
				get { return _pieceSpans[_pieceIndex]; }
			}
		}

		public int Length
		{
			get { return PieceCount == 0 ? 0 : LastPieceSpan.End; }
		}

		public int PieceCount
		{
			get { return _pieceSpans.Length; }
		}

		public T[] ToArray()
		{
			var result = new T[Length];
			var destinationIndex = 0;
			foreach (var span in _pieceSpans)
			{
				var piece = span.Piece;
				var length = piece.Length;
				piece.CopyTo(result, destinationIndex, 0, length);
				destinationIndex += length;
			}
			return result;
		}

		public void CopyTo(T[] destination, int destinationIndex, int position, int length)
		{
			CheckPosition(position, Length);

			var pieceIndex = PieceIndexForPosition(position);
			while (pieceIndex < PieceCount)
			{
				var span = _pieceSpans[pieceIndex];
				var positionInPiece = position - span.Start;
				var remainingLengthInPiece = span.Length - positionInPiece;
				var lengthToCopy = Math.Min(remainingLengthInPiece, length);
				span.Piece.CopyTo(destination, destinationIndex, positionInPiece, lengthToCopy);
				length -= lengthToCopy;
				if (length <= 0)
					break;
				destinationIndex += lengthToCopy;
				position += lengthToCopy;
				++pieceIndex;
			}
		}

		public IPiece<T> Range(int index, int length)
		{
			return Piece.ForRange(this, index, length);
		}

		public PieceTable<T> Insert(int position, T value)
		{
			CheckPosition(position, Length);

			if (position == Length)
				return position == 0 ? InsertFirst(value) : Append(value);

			return new Insertion(this, position, value).Run();
		}

		public PieceTable<T> Delete(int position, int length)
		{
			CheckPosition(position, Length - 1);

			var startingPieceIndex = PieceIndexForPosition(position);
			var startingPieceSpan = _pieceSpans[startingPieceIndex];
			var startingPiece = startingPieceSpan.Piece;

			var positionInStartingPiece = position - startingPieceSpan.Start;
			var remainingStartingPieceRange = startingPiece.Range(0, positionInStartingPiece);
			var newStartingPieceSpan = PieceSpan.For(remainingStartingPieceRange, startingPieceSpan.Start);

			var firstIndexAfterDeletion = position + length;
			if (firstIndexAfterDeletion >= Length)
				return ReplacePieceSpans(startingPieceIndex, PieceCount - startingPieceIndex, newStartingPieceSpan);

			var endingPieceIndex = firstIndexAfterDeletion < startingPieceSpan.End
				? startingPieceIndex
				: PieceIndexForPosition(firstIndexAfterDeletion, startingPieceIndex);

			var endingPieceSpan = _pieceSpans[endingPieceIndex];
			var endingPiece = endingPieceSpan.Piece;

			var positionInEndingPiece = firstIndexAfterDeletion - endingPieceSpan.Start;
			var remainingEndingPieceRange = endingPiece.Range(positionInEndingPiece, endingPiece.Length - positionInEndingPiece);
			var newEndingPieceSpan = PieceSpan.For(remainingEndingPieceRange, newStartingPieceSpan.End);

			return ReplacePieceSpans(
				startingPieceIndex,
				endingPieceIndex - startingPieceIndex + 1,
				newStartingPieceSpan, newEndingPieceSpan);
		}

		private void CheckPosition(int position, int maxValue)
		{
			if (position < 0 || position > maxValue)
				throw new ArgumentOutOfRangeException("position", position, string.Format("position must be between 0 and {0}", maxValue));
		}

		private PieceTable<T> InsertFirst(T value)
		{
			return NewPieceTableWith(NewSpanWith(value, 0));
		}

		private PieceTable<T> Append(T value)
		{
			return Piece.IsAppendable(LastPiece)
				? AppendTo(LastIndex, value)
				: AppendNew(value);
		}

		private PieceTable<T> AppendTo(int pieceIndex, T value)
		{
			var pieceSpan = _pieceSpans[pieceIndex];
			var appendablePiece = (IAppendablePiece<T>)pieceSpan.Piece;
			var appended = PieceSpan.For(appendablePiece.Append(value), pieceSpan.Start);
			return ReplacePieceSpan(pieceIndex, appended);
		}

		private PieceTable<T> AppendNew(T value)
		{
			var newPieceSpans = _pieceSpans.Append(NewSpanWith(value, Length));
			return NewPieceTableWith(newPieceSpans);
		}

		private PieceSpan<T> NewSpanWith(T value, int start)
		{
			return PieceSpan.For(NewPieceWith(value), start);
		}

		private IPiece<T> NewPieceWith(T value)
		{
			return _insertionBuffer.Insert(value);
		}

		private int PieceIndexForPosition(int position, int min = 0)
		{
			var max = LastIndex;
			while (max >= min)
			{
				var pivot = (min + max) / 2;
				var partition = _pieceSpans[pivot];
				if (position >= partition.Start)
				{
					if (position < partition.End)
						return pivot;
					min = pivot + 1;
					continue;
				}
				max = pivot - 1;
			}
			throw new InvalidOperationException();
		}

		private PieceTable<T> NewPieceTableWith(params PieceSpan<T>[] newPieceSpans)
		{
			var newPieceTable = new PieceTable<T>(newPieceSpans, _insertionBuffer);
			return newPieceTable.PieceCount >= PartitionThreshold
				? PartitionedPieceTableFor(newPieceTable)
				: newPieceTable;
		}

		private static PieceTable<T> PartitionedPieceTableFor(PieceTable<T> p)
		{
			var pivot = p.Length / 2;
			var parts = p.SplitAt(pivot);
			var first = PieceSpan.For(parts.First, 0);
			var second = PieceSpan.For(parts.Second, pivot);
			return new PieceTable<T>(new[] {first, second}, new InsertionBuffer<T>());
		}

		private static void UpdateSpans(PieceSpan<T>[] spans, int startingIndex)
		{
			for (var i = startingIndex; i < spans.Length; ++i)
			{
				var span = spans[i];
				spans[i] = PieceSpan.For(span.Piece, spans[i - 1].End);
			}
		}

		private PieceTable<T> ReplacePieceSpan(int pieceIndex, params PieceSpan<T>[] replacementPieces)
		{
			return ReplacePieceSpans(pieceIndex, 1, replacementPieces);
		}

		private PieceTable<T> ReplacePieceSpans(int pieceIndex, int count, params PieceSpan<T>[] replacementPieces)
		{
			var nonEmptySpans = NonEmptySpans(replacementPieces);
			var newPieceSpans = _pieceSpans.ReplaceRange(pieceIndex, count, nonEmptySpans);
			UpdateSpans(newPieceSpans, pieceIndex + nonEmptySpans.Length);
			return NewPieceTableWith(newPieceSpans);
		}

		private static PieceSpan<T>[] NonEmptySpans(PieceSpan<T>[] spans)
		{
			var nonEmptyCount = NonEmptyCount(spans);
			return nonEmptyCount == spans.Length
				? spans
				: NonEmptySpans(spans, nonEmptyCount);
		}

		private static PieceSpan<T>[] NonEmptySpans(PieceSpan<T>[] spans, int nonEmptyCount)
		{
			var nonEmpty = new PieceSpan<T>[nonEmptyCount];

			var nonEmptyIndex = 0;
			foreach (var span in spans)
				if (span.Length > 0)
					nonEmpty[nonEmptyIndex++] = span;

			return nonEmpty;
		}

		private static int NonEmptyCount(PieceSpan<T>[] spans)
		{
			var nonEmptyCount = 0;
// ReSharper disable LoopCanBeConvertedToQuery
			foreach (var span in spans)
// ReSharper restore LoopCanBeConvertedToQuery
				if (span.Length > 0)
					++nonEmptyCount;
			return nonEmptyCount;
		}

		private IPiece<T> LastPiece
		{
			get { return LastPieceSpan.Piece; }
		}

		private PieceSpan<T> LastPieceSpan
		{
			get { return _pieceSpans[LastIndex]; }
		}

		private int LastIndex
		{
			get { return _pieceSpans.Length - 1; }
		}

		struct Insertion
		{
			private readonly PieceTable<T> _table;
			private readonly int _position;
			private readonly T _value;
			private readonly int _pieceIndex;
			private readonly PieceSpan<T>[] _pieceSpans;
			private readonly PieceSpan<T> _pieceSpan;
			private readonly IPiece<T> _piece;

			public Insertion(PieceTable<T> table, int position, T value)
			{
				_pieceIndex = table.PieceIndexForPosition(position);
				_table = table;
				_position = position;
				_value = value;
				_pieceSpans = _table._pieceSpans;
				_pieceSpan = _pieceSpans[_pieceIndex];
				_piece = _pieceSpan.Piece;
			}

			public PieceTable<T> Run()
			{
				return CanInsertByAppendingToPreviousPiece()
					? InsertByAppending()
					: InsertBySplitting();
			}

			private bool CanInsertByAppendingToPreviousPiece()
			{
				return InsertingAtTheBeginning
					&& HasPreviousPiece
					&& PreviousPieceIsAppendable;
			}

			private bool InsertingAtTheBeginning
			{
				get { return _position == _pieceSpan.Start; }
			}

			private bool HasPreviousPiece
			{
				get { return _pieceIndex > 0; }
			}

			private bool PreviousPieceIsAppendable
			{
				get { return Piece.IsAppendable(Previous.Piece); }
			}

			private PieceSpan<T> Previous
			{
				get { return _pieceSpans[PreviousIndex]; }
			}

			private int PreviousIndex
			{
				get { return _pieceIndex - 1; }
			}

			private PieceTable<T> InsertByAppending()
			{
				return _table.AppendTo(PreviousIndex, _value);
			}

			private PieceTable<T> InsertBySplitting()
			{
				var pieceStart = _pieceSpan.Start;
				var positionInPiece = _position - pieceStart;
				var parts = _piece.SplitAt(positionInPiece);
				var replacementPieces = new[]
        {
		PieceSpan.For(parts.First, pieceStart),
		_table.NewSpanWith(_value, _position),
		PieceSpan.For(parts.Second, _position + 1)
        };
				return _table.ReplacePieceSpan(_pieceIndex, replacementPieces);
			}
		}
	}
}
