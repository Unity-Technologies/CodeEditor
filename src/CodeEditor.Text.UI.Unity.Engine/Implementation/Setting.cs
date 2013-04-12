using System;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public abstract class Setting : ISetting
	{
		protected IPreferences _preferences;

		public Setting(string id, ISettings settings)
		{
			ID = id;
			if (settings != null)
			{
				_preferences = settings.Preferences;
				settings.AddSetting(id, this);
			}
		}

		public string ID { get; set; }

		public event EventHandler Changed;

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}
	}

	public class IntSetting : Setting
	{
		int _value;

		public IntSetting(string id, int defaultValue, ISettings settings)
			: base(id, settings)
		{
			if (_preferences != null)
				_value = _preferences.GetInt(ID, defaultValue);
		}

		public int Value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					_value = value;
					if (_preferences != null)
						_preferences.SetInt(ID, _value);
					OnChanged();
				}
			}
		}
	}

	public class BoolSetting : Setting
	{
		bool _value;

		public BoolSetting(string id, bool defaultValue, ISettings settings)
			: base(id, settings)
		{
			if (_preferences != null)
				_value = _preferences.GetBool(ID, defaultValue);
		}

		public bool Value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					_value = value;
					if (_preferences != null)
						_preferences.SetBool(ID, _value);
					OnChanged();
				}
			}
		}
	}

	public class StringSetting : Setting
	{
		string _value;

		public StringSetting(string id, string defaultValue, ISettings settings)
			: base(id, settings)
		{
			if (_preferences != null)
				_value = _preferences.GetString(ID, defaultValue);
		}

		public string Value
		{
			get { return _value; }
			set
			{
				if (value != _value)
				{
					_value = value;
					if (_preferences != null)
						_preferences.SetString(ID, _value);
					OnChanged();
				}
			}
		}
	}
}