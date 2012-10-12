namespace CodeEditor.Text.Logic.Implementation
{
	public class Classification : IClassification
	{
		private readonly string _description;

		public Classification(string description)
		{
			_description = description;
		}

		public override string ToString()
		{
			return _description;
		}
	}
}