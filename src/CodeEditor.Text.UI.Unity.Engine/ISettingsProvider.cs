namespace CodeEditor.Text.UI.Unity.Engine
{
	interface ISettingsProvider
	{
		ISettings GetSettings(IPreferences preferences);
	}
}
