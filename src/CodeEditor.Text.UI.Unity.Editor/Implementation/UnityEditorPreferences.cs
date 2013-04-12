using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IPreferences))]
	class UnityEditorPreferences : IPreferences
	{
		public void SetInt(string key, int value)
		{
			EditorPrefs.SetInt(key, value);
		}

		public int GetInt(string key, int defaultValue)
		{
			return EditorPrefs.GetInt(key, defaultValue);
		}

		public void SetBool(string key, bool value)
		{
			EditorPrefs.SetBool(key, value);
		}

		public bool GetBool(string key, bool defaultValue)
		{
			return EditorPrefs.GetBool(key, defaultValue);
		}

		public void SetString(string key, string value)
		{
			EditorPrefs.SetString(key, value);
		}

		public string GetString(string key, string defaultValue)
		{
			return EditorPrefs.GetString(key, defaultValue);
		}
	}
}