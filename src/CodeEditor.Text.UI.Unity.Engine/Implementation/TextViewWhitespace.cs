using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewWhitespace : ITextViewWhitespace
	{
		int _spacesPerTab = 4;
		string _dot = "\u00B7";
		char _rightArrow = '\u2192';
		string _tabStringWithRightArrow;
		string[] _spaces;
		bool _initialized;

		public int NumberOfSpacesPerTab
		{
			get 
			{
				return _spacesPerTab;
			}
			set 
			{
				if (_spacesPerTab != value)
				{
					_spacesPerTab = Mathf.Max(value, 1);
					Init();
				}
			}
		}

		void Init()
		{
			_spaces = new string[_spacesPerTab + 1];
			for (int i = 1; i <= _spacesPerTab; ++i)
				_spaces[i] = new string(' ', i);

			StringBuilder sb = new StringBuilder(new string(' ', _spacesPerTab));
			sb[0] = _rightArrow;
			_tabStringWithRightArrow = sb.ToString();

			_initialized = true;
		}

		public bool Visible { get; set; }

		public List<int> GetTabSizes(string baseText)
		{
			List<int> tabSizes;
			FormatBaseText(baseText, out tabSizes);
			return tabSizes;
		}

		public string FormatBaseText(string baseText)
		{
			List<int> tabSizes;
			return FormatBaseText(baseText, out tabSizes);
		}

		public string FormatBaseText(string baseText, out List<int> tabSizes)
		{
			if (!_initialized)
				Init();

			tabSizes = new List<int>();

			StringBuilder sb = new StringBuilder(baseText.Replace("\t", _tabStringWithRightArrow));
			for (int i=0; i<sb.Length; ++i)
			{
				if (sb[i] == _rightArrow)
				{
					int wantedSpaces = TabStopSpaces(i);
					int removeCount = NumberOfSpacesPerTab - wantedSpaces;
					if (removeCount > 0)
						sb.Remove (i+1, removeCount);
					if (!Visible)
						sb[i] = ' ';
					tabSizes.Add (wantedSpaces);
				}
			}
			return sb.ToString();
		}

		int TabStopSpaces(int pos)
		{
			return NumberOfSpacesPerTab - ((pos) % NumberOfSpacesPerTab);
		}

		public string FormatRichText(string richText, List<int> tabSizes)
		{
			if (Visible)
				richText = richText.Replace(" ", _dot);

			StringBuilder sb = new StringBuilder();
			int startIndex = 0;
			int tabCounter = 0;
			while (startIndex < richText.Length)
			{
				int pos = richText.IndexOf('\t', startIndex);
				if (pos >= 0)
				{
					if (pos > startIndex)
						sb.Append(richText.Substring(startIndex, pos - startIndex));
					int insertNumSpaces = tabSizes[tabCounter++];
					sb.Append(_spaces[insertNumSpaces]);
					if (Visible)
						sb[sb.Length - insertNumSpaces] = _rightArrow;
					startIndex = pos + 1;
				}
				else
				{
					sb.Append(richText.Substring(startIndex));
					break;
				}
			}
			return sb.ToString();
		}

		public int ConvertToLogicalCaretColumn(int graphicalCaretColumn, ITextViewLine line)
		{
			return ConvertToLogicalCaretColumn(graphicalCaretColumn, line, GetTabSizes(line.Text));
		}

		public int ConvertToLogicalCaretColumn(int graphicalCaretColumn, ITextViewLine line, List<int> tabSizes)
		{
			string text = line.Text;
			int glyphCounter = 0;
			int tabCounter = 0;
			for (int i = 0; i <= text.Length; i++)
			{
				if (glyphCounter >= graphicalCaretColumn)
					return i;

				if (text[i] == '\t')
				{
					int numSpacesToInsert = tabSizes[tabCounter++];
					if (glyphCounter + numSpacesToInsert / 2 >= graphicalCaretColumn)
						return i;
					glyphCounter += numSpacesToInsert;
				}
				else
				{
					glyphCounter += 1;
				}
			}
			return -1;
		}

		public int ConvertToGraphicalCaretColumn(int logicalCaretColumn, ITextViewLine line)
		{
			return ConvertToGraphicalCaretColumn(logicalCaretColumn, line, GetTabSizes(line.Text));
		}

		public int ConvertToGraphicalCaretColumn(int logicalCaretColumn, ITextViewLine line, List<int> tabSizes)
		{
			int glyphCounter = 0;
			int tabCounter = 0;
			for (int i = 0; i <= line.Text.Length; i++)
			{
				if (i == logicalCaretColumn)
					return glyphCounter;
				glyphCounter += (line.Text[i] == '\t') ? tabSizes[tabCounter++] : 1;
			}
			return -1;
		}

	}
}
