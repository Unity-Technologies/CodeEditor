using CodeEditor.IO;
using CodeEditor.Text.Data;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewDocument
	{
		int LineCount { get; }
		ITextViewLine Line(int index);
		ITextViewLine CurrentLine { get; }
		ICaret Caret { get; }
		ITextBuffer Buffer { get; }
		IFile File { get; }
		IClassificationStyler ClassificationStyler {get;}
		void Save();
	}

	public static class TextViewDocumentExtensions
	{
		public static void Insert(this ITextViewDocument document, int start, string text)
		{
			document.Buffer.Insert(start, text);
		}

		public static void Append(this ITextViewDocument document, string text)
		{
			document.Buffer.Append(text);
		}

		public static void AppendLine(this ITextViewDocument document, string text)
		{
			document.Append(text + "\n");
		}

		public static void Delete(this ITextViewDocument document, int start, int length)
		{
			document.Buffer.Delete(start, length);
		}

		public static void Delete(this ITextViewDocument document, Span span)
		{
			document.Buffer.Delete(span.Start, span.Length);
		}

		public static void DeleteLine(this ITextViewDocument document, int lineNumber)
		{
			document.Delete(document.Buffer.CurrentSnapshot.Lines[lineNumber].ExtentIncludingLineBreak.Span);
		}

		public static ITextViewLine LastLine(this ITextViewDocument document)
		{
			return document.Line(document.LastLineIndex());
		}

		public static int LastLineIndex(this ITextViewDocument document)
		{
			return document.LineCount - 1;
		}
	}
}
