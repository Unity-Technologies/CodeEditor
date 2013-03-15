using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewWhitespace : ITextViewWhitespace
	{
		int _spacesPerTab = 4;
		string _dot = "\u00B7";
		char _rightArrow = '\u2192';
		string _tabString = " ";
		string _tabStringWithVisibleWhitespace = " ";

		public int NumberOfSpacesPerTab
		{
			get {return _spacesPerTab;}
			set {_spacesPerTab = Mathf.Max(value, 1);}
		}

		public bool Visible { get; set; }

		public string ReplaceWhitespace(string text)
		{
			if (Visible)
			{
				text = text.Replace(" ", _dot);
				text = text.Replace("\t", TabAsVisibleSpaces);
			}
			else
			{
				text = text.Replace("\t", TabAsSpaces);
			}
			return text;
		}

		string TabAsSpaces
		{
			get
			{
				if (_tabString.Length != NumberOfSpacesPerTab)
				{
					_tabString = new string(' ', NumberOfSpacesPerTab);
				}
				return _tabString;
			}
		}

		string TabAsVisibleSpaces
		{
			get
			{
				if (_tabStringWithVisibleWhitespace.Length != NumberOfSpacesPerTab)
				{
					StringBuilder sb = new StringBuilder(TabAsSpaces);
					sb[0] = _rightArrow;
					_tabStringWithVisibleWhitespace = sb.ToString();
				}
				return _tabStringWithVisibleWhitespace;
			}
		}
	}
}
