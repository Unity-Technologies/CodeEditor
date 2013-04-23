using System;
using System.Text;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	class CodeCompletionTextFormatter
	{
		readonly string _autoCompleteWord;

		public CodeCompletionTextFormatter (string autoCompleteWord)
		{
			_autoCompleteWord = autoCompleteWord;
		}

		public bool CanBeAutoCompleted (string target)
		{
			if (HasWordMatch(_autoCompleteWord, target))
			{
				return true;
			}

			if (HasLetterMatch(_autoCompleteWord, target))
			{
				return true;
			}

			return false;
		}

		static bool HasWordMatch(string autoWord, string target)
		{
			int pos = target.IndexOf(autoWord, StringComparison.OrdinalIgnoreCase);
			if (pos >= 0)
			{
				if (char.IsUpper(target[pos]) || pos == 0)
					return true;
			}
			return false;
		}

		static bool HasLetterMatch(string autoWord, string target)
		{
			StringBuilder stringBuilder = new StringBuilder(10);
			stringBuilder.Append(target[0]); // always add first char as we will try to match it uppercase or not
			for (int t = 1; t < target.Length; ++t)
				if (char.IsUpper(target[t]))
					stringBuilder.Append(target[t]);

			string compare = stringBuilder.ToString();
			if (compare.IndexOf(autoWord, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}

			return false;
		}

		static string CreateWordMatchRichText(string autoWord, string text, Color colorOfAutoCompleteWord)
		{
			var textBuilder = new StringBuilder();

			int position = text.IndexOf(autoWord, StringComparison.OrdinalIgnoreCase);
			if (position > 0)
				textBuilder.Append(text.Substring(0, position));
			textBuilder.Append("<color=#");
			textBuilder.Append(HexifyColor(colorOfAutoCompleteWord));
			textBuilder.Append(">");
			textBuilder.Append(text.Substring(position, autoWord.Length));
			textBuilder.Append("</color>");
			textBuilder.Append(text.Substring(position + autoWord.Length));
			
			return textBuilder.ToString();			
		}

		public string CreateRichText(string text, Color colorOfAutoCompleteWord)
		{
			if (HasWordMatch(_autoCompleteWord, text))
			{
				return CreateWordMatchRichText(_autoCompleteWord, text, colorOfAutoCompleteWord);
			}

			if (HasLetterMatch(_autoCompleteWord, text))
			{
				return CreateLetterMatchRichText(_autoCompleteWord, text, colorOfAutoCompleteWord);
			}

			return text;
		}

		static string CreateLetterMatchRichText(string autoWord, string target, Color colorOfAutoCompleteWord)
		{
			StringBuilder textBuilder = new StringBuilder(10);

			int t = 0;
			for (int a = 0; a < autoWord.Length; ++a)
			{
				for (; t < target.Length; ++t)
				{
					if (CompareChars(autoWord[a], target[t], t == 0))
					{
						textBuilder.Append("<color=#");
						textBuilder.Append(HexifyColor(colorOfAutoCompleteWord));
						textBuilder.Append(">");
						textBuilder.Append(target[t]);
						textBuilder.Append("</color>");
						t++;
						break;
					}
					else
					{
						textBuilder.Append(target[t]);
					}
				}
			}
			if (t < target.Length)
				textBuilder.Append(target.Substring(t));

			return textBuilder.ToString();
		}

		static bool CompareChars(char a, char b, bool ignoreCase)
		{
			if (ignoreCase && CompareCharsCaseInsensitive(a, b))
				return true;
			if (char.IsUpper(b) && CompareCharsCaseInsensitive(a, b))
				return true;
			return false;
		}

		static bool CompareCharsCaseInsensitive(char a, char b)
		{
			return char.ToLower(a) == char.ToLower(b);
		}

		static string HexifyColor(Color color)
		{
			return HexifyColorComponent(color.r) + HexifyColorComponent(color.g) + HexifyColorComponent(color.b);
		}

		static string HexifyColorComponent(float c)
		{
			return Mathf.FloorToInt(c * 255).ToString("x2");
		}
	}
}
