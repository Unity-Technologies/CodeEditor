using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IFontManagerProvider))]
	class FontManagerProvider : IFontManagerProvider
	{
		public IFontManager GetFontManager(ISettings settings)
		{
			string defaultFontName = "SourceCodePro-Regular";
			int defaultSize = 14;
			var _currentFontName = new StringSetting("CurrentFontName", defaultFontName, settings);
			var _currentFontSize = new IntSetting("CurrentFontSize", defaultSize, settings);
			return new FontManager(_currentFontName, _currentFontSize);
		}
	}
}
