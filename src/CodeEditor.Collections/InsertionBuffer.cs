using System;
using System.Collections.Generic;

namespace CodeEditor.Collections
{
	public class InsertionBuffer<T>
	{
		private readonly List<T> _buffer = new List<T>();

		public IAppendablePiece<T> Insert(T value)
		{
			var piece = new Piece(_buffer, _buffer.Count, 1);
			_buffer.Add(value);
			return piece;
		}

		class Piece : IAppendablePiece<T>
		{
			private readonly List<T> _buffer;
			private readonly int _count;
			private readonly int _index;

			public Piece(List<T> buffer, int index, int count)
			{
				_buffer = buffer;
				_index = index;
				_count = count;
			}

			public int Length
			{
				get { return _count; }
			}

			public bool IsAppendable
			{
				get { return _index  + _count == _buffer.Count; }
			}

			public void CopyTo(T[] destination, int destinationIndex, int sourceIndex, int length)
			{
				_buffer.CopyTo(_index + sourceIndex, destination, destinationIndex, length);
			}

			public IPiece<T> Range(int index, int length)
			{
				return new Piece(_buffer, _index + index, length);
			}

			public T this[int index]
			{
				get { return _buffer[_index + index]; }
			}

			public IPiece<T> Append(T value)
			{
				if (!IsAppendable)
					throw new InvalidOperationException();
				_buffer.Add(value);
				return new Piece(_buffer, _index, _count + 1);
			}
		}
	}
}
