using System.Collections.Generic;
using System.Linq;
using Boo.Ide.Grammars;
using CodeEditor.Text.Data;

namespace CodeEditor.Languages.Common
{
	public interface IPartitionTokenizer
	{
		IEnumerable<PartitionToken> Tokenize(PartitionTokenType previousPartitionTokenType, string text);
	}

	public class PartitionTokenTypeCache
	{
		private readonly IPartitionTokenizer _partitioner;
		private readonly List<PartitionTokenType> _previousTokenTypeForLine;

		public PartitionTokenTypeCache(IPartitionTokenizer partitioner)
		{
			_partitioner = partitioner;
			_previousTokenTypeForLine = new List<PartitionTokenType> { PartitionTokenType.None };
		}

		public void InvalidateFrom(int lineNumber)
		{
			var firstAffectedLine = lineNumber + 1;
			var knownLines = _previousTokenTypeForLine.Count;
			if (firstAffectedLine >= knownLines)
				return;
			_previousTokenTypeForLine.RemoveRange(firstAffectedLine, knownLines - firstAffectedLine);
		}

		public PartitionTokenType LastPartitionTokenTypeBefore(ITextSnapshotLine line)
		{
			EnsurePartitionsUpTo(line);
			return _previousTokenTypeForLine[line.LineNumber];
		}

		private void EnsurePartitionsUpTo(ITextSnapshotLine line)
		{
			var lastKnownLineNumber = _previousTokenTypeForLine.Count - 1;
			if (line.LineNumber <= lastKnownLineNumber)
				return;

			var lastKnownTokenType = _previousTokenTypeForLine[lastKnownLineNumber];
			var lines = line.Snapshot.Lines;
			for (var i = lastKnownLineNumber + 1; i <= line.LineNumber; ++i)
			{
				var precedingLine = lines[i - 1];
				if (precedingLine.Text.Length != 0)
				{
					var lastTokenTypeOfPrecedingLine = PartitionTokensFor(precedingLine, lastKnownTokenType).Last().Type;
					lastKnownTokenType = IsLineComment(lastTokenTypeOfPrecedingLine)
						? PartitionTokenType.None
						: lastTokenTypeOfPrecedingLine;
				}
				_previousTokenTypeForLine.Add(lastKnownTokenType);
			}
		}

		bool IsLineComment(PartitionTokenType tokenType)
		{
			return tokenType == PartitionTokenType.CommentLineContents || tokenType == PartitionTokenType.CommentLineBegin;
		}

		private IEnumerable<PartitionToken> PartitionTokensFor(ITextSnapshotLine line, PartitionTokenType previousPartitionTokenType)
		{
			return _partitioner.Tokenize(previousPartitionTokenType, line.Text);
		}
	}
}