using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(IDefaultTextViewMarginsProvider))]
	class DefaultTextViewMarginsProvider : IDefaultTextViewMarginsProvider
	{
		[ImportMany]
		public ITextViewMarginProvider[] MarginProviders { get; set; }

		public ITextViewMargins MarginsFor(ITextView textView)
		{
			return new TextViewMargins(MarginProviders.Select(p => p.MarginFor(textView)).Where(m => m!=null));
		}
	}
}
