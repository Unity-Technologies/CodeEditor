using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public class TextViewWhitespace : ITextViewWhitespace
	{
		char _visibleSpaceChar = '\u00B7';
		char _visibleTabChar = '\u2192';
		string _tabStringWithRightArrow;
		string[] _spaces;
		bool _initialized;
		BoolSetting _showWhitespace;
		IntSetting _numSpacesPerTab;

		public TextViewWhitespace(ISettings settings, BoolSetting showWhitespace, IntSetting numSpacesPerTab)
		{
			_showWhitespace = showWhitespace;
			_numSpacesPerTab = numSpacesPerTab;
			_numSpacesPerTab.Changed += (sender, args) => Init();

			if (_numSpacesPerTab.Value < 1)
				_numSpacesPerTab.Value = 1;
		}

		void Init()
		{
			_numSpacesPerTab.Value = Mathf.Max(_numSpacesPerTab.Value, 1);

			_spaces = new string[_numSpacesPerTab.Value + 1];
			for (int i = 1; i <= _numSpacesPerTab.Value; ++i)
				_spaces[i] = new string(' ', i);

			StringBuilder sb = new StringBuilder(new string(' ', _numSpacesPerTab.Value));
			sb[0] = _visibleTabChar;
			_tabStringWithRightArrow = sb.ToString();

			_initialized = true;
		}

		public char VisibleSpaceChar { get { return _visibleSpaceChar; } set { _visibleSpaceChar = value; } }
		public char VisibleTabChar { get { return _visibleTabChar; } set { _visibleTabChar = value; Init(); } }
		public bool Visible { get { return _showWhitespace.Value;} set{ _showWhitespace.Value = value;} }
		public int NumberOfSpacesPerTab { get { return _numSpacesPerTab.Value; } set { _numSpacesPerTab.Value = value; } }

		int TabStopSpaces(int pos)
		{
			return NumberOfSpacesPerTab - ((pos) % NumberOfSpacesPerTab);
		}

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

			if (Visible)
				baseText = baseText.Replace(" ", new string(VisibleSpaceChar, 1));

			StringBuilder sb = new StringBuilder(baseText.Replace("\t", _tabStringWithRightArrow));
			for (int i=0; i<sb.Length; ++i)
			{
				if (sb[i] == VisibleTabChar)
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

		public string FormatRichText(string richText, List<int> tabSizes)
		{
			if (Visible)
				richText = richText.Replace(" ", new string(VisibleSpaceChar,1));

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
						sb[sb.Length - insertNumSpaces] = VisibleTabChar;
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
				if (glyphCounter >= graphicalCaretColumn || i == text.Length)
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
			if (logicalCaretColumn < 0 || logicalCaretColumn > line.Text.Length)
			{
				Debug.LogError("Invalid input: Ensure 0 <= logicalCaretColumn <= line.Text.Length (logicalCaretColumn: " + logicalCaretColumn + ", text length " + line.Text.Length + ")");
				return 0;
			}

			int glyphCounter = 0;
			int tabCounter = 0;
			int textLength = line.Text.Length;
			for (int i = 0; i <= textLength; i++)
			{
				if (i == logicalCaretColumn)
					return glyphCounter;
				glyphCounter += (line.Text[i] == '\t') ? tabSizes[tabCounter++] : 1;
			}
			return -1;
		}

	}
}
