using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEditor.Text.UI.Unity.Engine
{
	interface ISettingsProvider
	{
		ISettings GetSettings(IPreferences preferences);
	}
}
