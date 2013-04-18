using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewMarginProvider))]
	class LineNumberMarginProvider : ITextViewMarginProvider
	{
		public ITextViewMargin MarginFor(ITextView textView)
		{
			var visibilitySetting = new BoolSetting ("LineNumberVisiblitySetting", true, textView.Settings);
			return new LineNumberMargin(textView, visibilitySetting);
		}
	}
}
