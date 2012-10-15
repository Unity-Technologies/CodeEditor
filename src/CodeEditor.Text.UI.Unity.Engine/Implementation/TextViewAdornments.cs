using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Composition;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewAdornments))]
	class TextViewAdornments : ITextViewAdornments
	{
		[ImportMany] private ITextViewAdornment[] _adornments;

		public void Draw(ITextViewLine line, Rect lineRect)
		{
			foreach (var adornment in _adornments)
				adornment.Draw(line, lineRect);
		}
	}
}
