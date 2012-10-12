using System;
using CodeEditor.Composition;
using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI.Implementation
{
	[Export(typeof(ITextStructureNavigator))]
	public class DefaultTextStructureNavigator : ITextStructureNavigator
	{
		public TextSpan GetSpanFor(int position, ITextSnapshot snapshot)
		{
			if (position < 0 || position > snapshot.Length)
				throw new ArgumentOutOfRangeException("position", position.ToString());

			if (position == snapshot.Length)
				return new TextSpan(snapshot, snapshot.Length, 0);

			var c = snapshot[position];

			if (PunctuationSymbolOrLineEnding(c))
				return new TextSpan(snapshot, position, 1);
			if (LetterDigitOrUnderscore(c))
				return CreateSpan(position, LetterDigitOrUnderscore, snapshot);
			if (WhiteSpace(c))
				return CreateSpan(position, WhiteSpace, snapshot);

			throw new NotImplementedException("Character not supported: " + (byte)c);
		}

		TextSpan CreateSpan(int position, Func<char, bool> predicate, ITextSnapshot snapshot)
		{
			var start = position;
			var end = position + 1;

			while (start > 0 && predicate(snapshot[start - 1]))
				--start;

			while (end < snapshot.Length && predicate(snapshot[end]))
				++end;

			return new TextSpan(snapshot, start, end - start);
		}

		bool LetterDigitOrUnderscore(char c)
		{
			return char.IsLetterOrDigit(c) || c == '_';
		}

		bool WhiteSpace(char c)
		{
			return c == ' ' || c == '\t';
		}

		bool PunctuationSymbolOrLineEnding(char c)
		{
			return c == '\n' || char.IsPunctuation(c) || char.IsSymbol(c);
		}

		public TextSpan GetNextSpanFor(TextSpan span)
		{
			if (span.End == span.Snapshot.Length)
				return new TextSpan(span.Snapshot, span.Snapshot.Length, 0);

			return GetSpanFor(span.End, span.Snapshot);
		}

		public TextSpan GetPreviousSpanFor(TextSpan span)
		{
			if (span.Start == 0)
				return new TextSpan(span.Snapshot, 0, 0);

			return GetSpanFor(span.Start - 1, span.Snapshot);
		}
	}
}
