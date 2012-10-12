namespace CodeEditor.Text.Logic
{
	public interface IStandardClassificationRegistry
	{
		IClassification Text { get; }
		IClassification Keyword { get; }
		IClassification WhiteSpace { get; }
		IClassification Identifier { get; }
		IClassification Operator { get; }
		IClassification String { get; }
		IClassification Punctuation { get; }
		IClassification Comment { get; }
		IClassification Number { get; }
	}
}