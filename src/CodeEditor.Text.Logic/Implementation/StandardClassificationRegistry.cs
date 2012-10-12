using CodeEditor.Composition;

namespace CodeEditor.Text.Logic.Implementation
{
	[Export(typeof(IStandardClassificationRegistry))]
	public class StandardClassificationRegistry : IStandardClassificationRegistry
	{
		private readonly Classification _text = new Classification("Text");
		private readonly Classification _keyword = new Classification("Keyword");
		private readonly Classification _whitespace = new Classification("WhiteSpace");
		private readonly Classification _identifier = new Classification("Identifier");
		private readonly Classification _operator = new Classification("Operator");
		private readonly Classification _field = new Classification("String");
		private readonly Classification _punctuation = new Classification("Punctuation");
		private readonly Classification _comment = new Classification("Comment");
		private readonly Classification _number = new Classification("Number");

		public IClassification Text
		{
			get { return _text; }
		}

		public IClassification Keyword
		{
			get { return _keyword; }
		}

		public IClassification WhiteSpace
		{
			get { return _whitespace; }
		}

		public IClassification Identifier
		{
			get { return _identifier; }
		}

		public IClassification Operator
		{
			get { return _operator; }
		}

		public IClassification String
		{
			get { return _field; }
		}

		public IClassification Punctuation
		{
			get { return _punctuation; }
		}

		public IClassification Comment
		{
			get { return _comment; }
		}

		public IClassification Number
		{
			get { return _number; }
		}
	}
}