using CodeEditor.Composition;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewAdornments))]
	class TextViewAdornments : ITextViewAdornments
	{
		[ImportMany]
		public ITextViewAdornment[] Adornments { get; set; }

		public void Draw(ITextViewLine line, Rect lineRect)
		{
			foreach (var adornment in Adornments)
				adornment.Draw(line, lineRect);
		}
	}
}
