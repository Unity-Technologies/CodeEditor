using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewMarginProvider))]
	class LineNumberMarginProvider : ITextViewMarginProvider
	{
		public ITextViewMargin MarginFor(ITextView textView)
		{
			return new LineNumberMargin(textView);
		}
	}
}