using System.IO;
using System.Collections.Generic;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	class CodeCompletionListItemProvider : IListItemProvider
	{
		readonly List<IListItem> _items = new List<IListItem>();
		readonly IIcons _icons;
		readonly CodeCompletionTextFormatter _textFormatter;
		const bool _debug = true;
		
		public CodeCompletionListItemProvider(string autoCompleteWord)
		{
			_textFormatter = new CodeCompletionTextFormatter(autoCompleteWord);

			_icons = UnityEditorCompositionContainer.GetExportedValue<IIcons>();

			// Todo: Transfer session data to CodeCompletionListItem items

			if (_debug)
			{
				// For test input we use icon names:
				Icons icons = _icons as Icons;
				string[] iconFilePaths = icons.GetIconFilePaths();
			
				_items = new List<IListItem>();
				for (int i = 0; i < iconFilePaths.Length; i++)
				{
					string iconFilename = Path.GetFileNameWithoutExtension(iconFilePaths[i % iconFilePaths.Length]);
					string iconType = iconFilename.Substring(12, iconFilename.Length - 12); // remove 'Icons.16x16.'
					string itemText = string.Format("{0}", iconType);
					if (_textFormatter.CanBeAutoCompleted(itemText))
					{
						_items.Add(new CodeCompletionListItem(itemText, iconFilename, null));						
					}
				}
			}
		}

		public string CreateRichText(string text, Color colorOfAutoCompleteWord)
		{
			return _textFormatter.CreateRichText(text, colorOfAutoCompleteWord);
		}

		public List<IListItem> GetList()
		{
			return _items;
		}
	}
}
