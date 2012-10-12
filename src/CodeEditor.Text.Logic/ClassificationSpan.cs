using CodeEditor.Text.Data;

namespace CodeEditor.Text.Logic
{
	public struct ClassificationSpan
	{
		public readonly IClassification Classification;
		public readonly TextSpan Span;

		public ClassificationSpan(IClassification classification, TextSpan span)
		{
			Classification = classification;
			Span = span;
		}

		public int Start
		{
			get { return Span.Start; }
		}

		public int Length
		{
			get { return Span.Length; }
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Classification, Span);
		}
	}
}
