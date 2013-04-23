
namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	class CodeCompletionListItem : IListItem
	{
		public string RichText { get; set; }
		public string Text { get; set; }
		public string ItemType { get; set; }
		
		public CodeCompletionListItem(string text, string itemType, CodeCompletionListItemProvider owner)
		{
			Text = text;
			ItemType = itemType;
		}
	}
}
