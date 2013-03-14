using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewTabs : ITextViewTabs
	{
		int _whiteSpacesPerTab = 4;
		string _dot = "\u00B7";
		char _rightArrow = '\u2192';
		string _tabString = " ";
		string _tabStringShowWhiteSpace = " ";

		public int NumberOfWhitespacesPerTab
		{
			get {return _whiteSpacesPerTab;}
			set {_whiteSpacesPerTab = Mathf.Max(value, 1);}
		}

		public string ReplaceTabsWithWhiteSpaces(string text, bool showWhitespaces)
		{
			if (showWhitespaces)
			{
				text = text.Replace(" ", _dot);
				text = text.Replace("\t", TabAsVisibleWhiteSpaces);
			}
			else
			{
				text = text.Replace("\t", TabAsWhiteSpaces);
			}
			return text;
		}

		string TabAsWhiteSpaces
		{
			get
			{
				if (_tabString.Length != NumberOfWhitespacesPerTab)
				{
					_tabString = new string(' ', NumberOfWhitespacesPerTab);
				}
				return _tabString;
			}
		}

		string TabAsVisibleWhiteSpaces
		{
			get
			{
				if (_tabStringShowWhiteSpace.Length != NumberOfWhitespacesPerTab)
				{
					StringBuilder sb = new StringBuilder(TabAsWhiteSpaces);
					sb[0] = _rightArrow;
					_tabStringShowWhiteSpace = sb.ToString();
				}
				return _tabStringShowWhiteSpace;
			}
		}
	}
}
