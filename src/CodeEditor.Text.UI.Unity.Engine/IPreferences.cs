// Wrap EditorPrefs or PlayerPrefs

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface IPreferences
	{
		void SetInt(string key, int value);
		int GetInt(string key, int defaultValue);

		void SetBool(string key, bool value);
		bool GetBool(string key, bool defaultValue);

		void SetString(string key, string value);
		string GetString(string key, string defaultValue);

	}
}
