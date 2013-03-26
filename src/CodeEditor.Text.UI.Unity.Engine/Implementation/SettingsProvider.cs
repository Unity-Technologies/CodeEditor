using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ISettingsProvider))]
	class SettingsProvider : ISettingsProvider
	{
		public ISettings GetSettings(IPreferences preferences)
		{
			return new Settings(preferences);
		}
	}
}
