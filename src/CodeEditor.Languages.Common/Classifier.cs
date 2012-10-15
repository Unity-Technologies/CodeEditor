using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Ide.Grammars;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.Common
{
	public abstract class Classifier : IClassifier, IPartitionTokenizer
	{
		private readonly IStandardClassificationRegistry _standardClassificationRegistry;
		private readonly HashSet<string> _keywords;
		private readonly PartitionTokenizer _partitioner = new PartitionTokenizer();
		private readonly Tokenizer _tokenizer = new Tokenizer();
		private readonly PartitionTokenTypeCache _partitionTokenTypeCache;

		protected Classifier(IStandardClassificationRegistry standardClassificationRegistry, HashSet<string> keywords, ITextBuffer buffer)
		{
			_standardClassificationRegistry = standardClassificationRegistry;
			_keywords = keywords;
			_partitionTokenTypeCache = new PartitionTokenTypeCache(this);
			buffer.Changed += OnBufferChanged;
		}

		void OnBufferChanged(object sender, TextChangeArgs args)
		{
			_partitionTokenTypeCache.InvalidateFrom(args.LineNumber);
		}

		public IEnumerable<ClassificationSpan> Classify(ITextSnapshotLine line)
		{
			return ClassificationSpans.Merge(ClassificationsFor(line, GetPreviousPartitionTokenType(line)));
		}

		PartitionTokenType GetPreviousPartitionTokenType(ITextSnapshotLine line)
		{
			return _partitionTokenTypeCache.LastPartitionTokenTypeBefore(line);
		}

		private IEnumerable<ClassificationSpan> ClassificationsFor(ITextSnapshotLine line, PartitionTokenType previousPartitionTokenType)
		{
			foreach (var partitionToken in PartitionTokensFor(line, previousPartitionTokenType))
			{
				var textSpan = TextSpanForPartitionOfLine(line, partitionToken);

				if (partitionToken.Type == PartitionTokenType.Code)
					foreach (var classificationSpan in ClassificationsFor(textSpan))
						yield return classificationSpan;
				else
					yield return new ClassificationSpan(ClassificationFor(partitionToken), textSpan);
			}
		}

		private static TextSpan TextSpanForPartitionOfLine(ITextSnapshotLine line, PartitionToken partitionToken)
		{
			return new TextSpan(line.Snapshot, new Span(line.Start + partitionToken.Begin, partitionToken.Length));
		}

		private IEnumerable<PartitionToken> PartitionTokensFor(ITextSnapshotLine line, PartitionTokenType previousPartitionTokenType)
		{
			return _partitioner.Tokenize(previousPartitionTokenType, line.Text);
		}

		private IEnumerable<ClassificationSpan> ClassificationsFor(TextSpan textSpan)
		{
			return _tokenizer.Tokenize(textSpan.Text).Select(t => ClassificationSpanFor(t, textSpan.Snapshot, textSpan.Span.Start));
		}

		private IClassification ClassificationFor(PartitionToken partition)
		{
			switch (partition.Type)
			{
				case PartitionTokenType.CommentBegin:
				case PartitionTokenType.CommentContents:
				case PartitionTokenType.CommentEnd:
				case PartitionTokenType.CommentLineBegin:
				case PartitionTokenType.CommentLineContents:
				case PartitionTokenType.CommentLineEnd:
					return _standardClassificationRegistry.Comment;
				case PartitionTokenType.StringBegin:
				case PartitionTokenType.StringContents:
				case PartitionTokenType.StringEnd:
				case PartitionTokenType.StringSingleBegin:
				case PartitionTokenType.StringSingleContents:
				case PartitionTokenType.StringSingleEnd:
					return _standardClassificationRegistry.String;
			}
			throw new ArgumentOutOfRangeException();
		}

		private ClassificationSpan ClassificationSpanFor(Token token, ITextSnapshot textBuffer, int offset)
		{
			var textSpan = textBuffer.GetSpan(offset + token.Begin, token.End - token.Begin);
			return new ClassificationSpan(ClassificationFor(token, textSpan), textSpan);
		}

		private IClassification ClassificationFor(Token token, TextSpan textSpan)
		{
			switch (token.Type)
			{
				case TokenType.WhiteSpace:
					return _standardClassificationRegistry.WhiteSpace;
				case TokenType.Identifier:
					return _keywords.Contains(textSpan.Text)
						? _standardClassificationRegistry.Keyword
						: _standardClassificationRegistry.Identifier;
				case TokenType.Punctuation:
					return _standardClassificationRegistry.Punctuation;
				case TokenType.Number:
					return _standardClassificationRegistry.Number;
				case TokenType.None:
					return _standardClassificationRegistry.Operator;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		IEnumerable<PartitionToken> IPartitionTokenizer.Tokenize(PartitionTokenType previousPartitionTokenType, string text)
		{
			return _partitioner.Tokenize(previousPartitionTokenType, text);
		}
	}
}
