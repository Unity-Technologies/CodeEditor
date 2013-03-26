using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewWhitespaceProvider))]
	class TextViewWhitespaceProvider : ITextViewWhitespaceProvider
	{
		public ITextViewWhitespace GetWhitespace(ISettings settings)
		{
			var visibleWhitespace = new BoolSetting("VisibleWhitespace", false, settings);
			var numSpacesPerTab = new IntSetting("NumSpacesPerTab", 4, settings);
			return new TextViewWhitespace(settings, visibleWhitespace, numSpacesPerTab);
		}
	}
}
