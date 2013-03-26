using System.Collections.Generic;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ISettings
	{
		void AddSetting(string id, ISetting setting);
		void RemoveSetting(string id, ISetting setting);
		void Clear();
		ISetting GetSetting(string id);
		List<string> GetListOfIDs();
		IPreferences Preferences { get; set; }
	}
}
