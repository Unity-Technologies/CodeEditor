using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface IFontManager
	{
		string[] GetSupportedFontNames();
		int[] GetCurrentFontSizes();
		Font CurrentFont { get; }
		string CurrentFontName { get; set; }
		int CurrentFontSize { get;set; }
		event EventHandler Changed;
	}
}
