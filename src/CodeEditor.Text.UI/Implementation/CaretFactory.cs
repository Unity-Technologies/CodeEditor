using CodeEditor.Composition;
using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI.Implementation
{
	[Export(typeof(ICaretFactory))]
	class CaretFactory : ICaretFactory
	{
		public ICaret CaretForBuffer(ITextBuffer buffer)
		{
			return new Caret(new BufferBounds(buffer));
		}

		class BufferBounds : ICaretBounds
		{
			private readonly ITextBuffer _buffer;

			public BufferBounds(ITextBuffer buffer)
			{
				_buffer = buffer;
			}

			public int Rows
			{
				get { return CurrentSnapshotLines.Count; }
			}

			public int ColumnsForRow(int row)
			{
				return CurrentSnapshotLines[row].Length;
			}

			private ITextSnapshotLines CurrentSnapshotLines
			{
				get { return CurrentSnapshot.Lines; }
			}

			private ITextSnapshot CurrentSnapshot
			{
				get { return _buffer.CurrentSnapshot; }
			}
		}
	}
}