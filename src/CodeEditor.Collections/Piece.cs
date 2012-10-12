using System;

namespace CodeEditor.Collections
{
	/// <summary>
	/// An immutable piece of information.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IPiece<T>
	{
		int Length { get; }
		void CopyTo(T[] destination, int destinationIndex, int sourceIndex, int length);
		IPiece<T> Range(int index, int length);
		T this[int index] { get; }
	}

	/// <summary>
	/// An immutable piece of information with support for
	/// fast appending semantics.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IAppendablePiece<T> : IPiece<T>
	{
		bool IsAppendable { get; }
		IPiece<T> Append(T element);
	}

	public static class Piece
	{
		public static IPiece<char> ForString(string s)
		{
			return new StringPiece(s);
		}

		public static IPiece<T> ForArray<T>(T[] array)
		{
			return new ArrayPiece<T>(array);
		}

		public static IPiece<T> ForRange<T>(IPiece<T> piece, int start, int length)
		{
			return new PieceRange<T>(piece, start, length);
		}

		public static bool IsAppendable<T>(IPiece<T> piece)
		{
			var appendablePiece = piece as IAppendablePiece<T>;
			return appendablePiece != null && appendablePiece.IsAppendable;
		}

		public static Pair<IPiece<T>, IPiece<T>> SplitAt<T>(this IPiece<T> piece, int position)
		{
			var previous = piece.Range(0, position);
			var next = piece.Range(position, piece.Length - position);
			return Pair.Create(previous, next);
		}

		public static T[] ToArray<T>(this IPiece<T> piece, int position, int length)
		{
			if (position + length > piece.Length)
				throw new ArgumentOutOfRangeException();

			var destination = new T[length];
			piece.CopyTo(destination, 0, position, length);
			return destination;
		}

		public static T[] ToArray<T>(this IPiece<T> piece)
		{
			return piece.ToArray(0, piece.Length);
		}

		class StringPiece : IPiece<char>
		{
			private readonly string _s;

			public StringPiece(string s)
			{
				_s = s;
			}

			public int Length
			{
				get { return _s.Length; }
			}

			public void CopyTo(char[] destination, int destinationIndex, int sourceIndex, int length)
			{
				_s.CopyTo(sourceIndex, destination, destinationIndex, length);
			}

			public IPiece<char> Range(int index, int length)
			{
				return new StringPiece(_s.Substring(index, length));
			}

			public char this[int index]
			{
				get { return _s[index]; }
			}
		}

		class ArrayPiece<T> : IPiece<T>
		{
			private readonly T[] _array;

			public ArrayPiece(T[] array)
			{
				_array = array;
			}

			public int Length
			{
				get { return _array.Length; }
			}

			public void CopyTo(T[] destination, int destinationIndex, int sourceIndex, int length)
			{
				Array.Copy(_array, sourceIndex, destination, destinationIndex, length);
			}

			public IPiece<T> Range(int index, int length)
			{
				return new PieceRange<T>(this, index, length);
			}

			public T this[int index]
			{
				get { return _array[index]; }
			}
		}

		class PieceRange<T> : IPiece<T>
		{
			private readonly IPiece<T> _piece;
			private readonly int _index;
			private readonly int _length;

			public PieceRange(IPiece<T> piece, int index, int length)
			{
				_piece = piece;
				_index = index;
				_length = length;
			}

			public int Length
			{
				get { return _length; }
			}

			public void CopyTo(T[] destination, int destinationIndex, int sourceIndex, int length)
			{
				_piece.CopyTo(destination, destinationIndex, _index + sourceIndex, length);
			}

			public IPiece<T> Range(int index, int length)
			{
				return new PieceRange<T>(_piece, _index + index, length);
			}

			public T this[int index]
			{
				get { return _piece[_index + index]; }
			}
		}
	}
}
