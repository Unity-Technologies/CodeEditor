using System;

namespace CodeEditor.Text.Data
{
	public delegate void TextChange(object sender, TextChangeArgs args);

	public class TextChangeArgs : EventArgs
	{
		public readonly ITextSnapshot OldSnapshot;
		public readonly Span OldSpan;
		public readonly ITextSnapshot NewSnapshot;
		public readonly Span NewSpan;

		private int? _lineNumber;

		public TextChangeArgs(ITextSnapshot oldSnapshot, Span oldSpan, ITextSnapshot newSnapshot, Span newSpan)
		{
			OldSnapshot = oldSnapshot;
			OldSpan = oldSpan;
			NewSnapshot = newSnapshot;
			NewSpan = newSpan;
		}

		public int LineNumber
		{
			get { return (_lineNumber ?? (_lineNumber = OldSnapshot.LineNumberForPosition(OldSpan.Start))).Value; }
		}

		public string OldText
		{
			get { return OldSnapshot.GetText(OldSpan); }
		}

		public string NewText
		{
			get { return NewSnapshot.GetText(NewSpan); }
		}

		public TextSpan NewTextSpan
		{
			get { return new TextSpan(NewSnapshot, NewSpan); }
		}
	}
}
