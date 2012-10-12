namespace CodeEditor.Text.UI.Completion.Implementation
{
	public class Completion : ICompletion
	{
		public Completion(string text)
		{
			Tooltip = DisplayText = text;
		}

		public Completion() {}

		public string DisplayText { get; set; }

		public string Tooltip { get; set; }
	}
}
