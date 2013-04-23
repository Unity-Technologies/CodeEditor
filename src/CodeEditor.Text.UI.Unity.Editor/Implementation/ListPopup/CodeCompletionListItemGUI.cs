using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	// Gui implementation for CodeCompletionListItem
	class CodeCompletionListItemGUI : IListItemGUI
	{
		GUIContent _guiContent = new GUIContent();
		GUIStyle _text;
		GUIStyle _textSelected;
		CodeCompletionListItemProvider _owner;
		Color _colorOfAutoCompleteWord = new Color(238 / 255f, 149 / 255f, 52 / 255f);

		IIcons _icons;

		public CodeCompletionListItemGUI(int itemHeight, CodeCompletionListItemProvider owner)
		{
			ItemHeight = itemHeight;
			_owner = owner;
			_icons = UnityEditorCompositionContainer.GetExportedValue<IIcons>();

			GUISkin skin = UnityEditorCompositionContainer.GetExportedValue<IGUISkinProvider>().GetGUISkin();
			_text = skin.GetStyle("ListText");
			_textSelected = skin.GetStyle("ListTextSelected");
			_text.richText = true;
			_textSelected.richText = true;
			
		}

		public float ItemHeight { get; set; }
		
		public void OnGUI(Rect rect, IListItem listItem, bool selected)
		{
			var item = listItem as CodeCompletionListItem;
			if (item == null)
			{
				Debug.LogError("item is not a CodeCompletionListItem");
				return;
			}
			
			const float iconSize = 16f;
			const float spaceBetween = 2f;
			const float margin = 5f;
			const float textOffset = margin + iconSize + spaceBetween;

			// Icon
			Texture2D icon = GetIconFor(item);
			if (icon != null)
				GUI.DrawTexture(new Rect(rect.x + margin, rect.y, iconSize, iconSize), icon);

			// Text
			InitRichTextFor(item);
			_guiContent.text = item.RichText;
			GUI.Label(new Rect(rect.x + textOffset, rect.y, rect.width - textOffset, rect.height), _guiContent, selected ? _textSelected : _text);
		}

		void InitRichTextFor(CodeCompletionListItem item)
		{
			if (string.IsNullOrEmpty(item.RichText))
				item.RichText = _owner.CreateRichText(item.Text, _colorOfAutoCompleteWord);
		}

		Texture2D GetIconFor(CodeCompletionListItem item)
		{
			string iconNameWithExension = item.ItemType + IconExtension;
			return _icons.GetIcon(iconNameWithExension);
		}

		string IconExtension {
			get { return ".png"; }
		}
	}
}
