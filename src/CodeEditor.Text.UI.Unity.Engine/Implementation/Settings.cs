using System.Collections.Generic;
using CodeEditor.Composition;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ISettings))]
	public class Settings : ISettings
	{
		Dictionary<string, ISetting> _settingsMap = new Dictionary<string, ISetting>();

		public Settings(IPreferences preferences)
		{
			Preferences = preferences;
		}

		public IPreferences Preferences { get; set; }

		public void AddSetting(string id, ISetting setting)
		{
			_settingsMap.Add(id, setting); // throws exception if key is already in the map, this way way we catch duplicate setting IDs.
		}

		public void RemoveSetting(string id, ISetting setting)
		{
			_settingsMap.Remove(id);
		}

		public void Clear()
		{
			_settingsMap.Clear();
		}

		public ISetting GetSetting(string id)
		{
			ISetting setting;
			if (_settingsMap.TryGetValue(id, out setting))
				return setting;
			return null;
		}

		public List<string> GetListOfIDs()
		{
			List<string> ids = new List<string>(_settingsMap.Count);
			foreach (KeyValuePair<string, ISetting> entry in _settingsMap)
				ids.Add(entry.Key);
			ids.Sort();
			return ids;
		}
	}
}
