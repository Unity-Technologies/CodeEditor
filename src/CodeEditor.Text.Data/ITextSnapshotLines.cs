using System.Collections.Generic;

namespace CodeEditor.Text.Data
{
	public interface ITextSnapshotLines : IEnumerable<ITextSnapshotLine>
	{
		int Count { get; }
		ITextSnapshotLine this[int lineNumber] { get; }
	}
}