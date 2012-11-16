
namespace CodeEditor.Text.UI
{

	[System.Serializable]
	public class FilePathProviderItem
	{
		public FilePathProviderItem (string displayText, object userData)
		{
			DisplayText = displayText;
			UserData = userData;
		}
		public string DisplayText { get; set; }
		public object UserData{ get; set; }
	}
}
