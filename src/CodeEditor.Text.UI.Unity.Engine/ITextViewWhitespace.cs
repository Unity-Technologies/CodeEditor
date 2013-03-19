
using System.Collections.Generic;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ITextViewWhitespace
	{
		int NumberOfSpacesPerTab { get; set; }
		bool Visible { get; set; }
		List<int> GetTabSizes(string baseText);
		string FormatBaseText(string baseText);
		string FormatBaseText(string baseText, out List<int> tabSizes);
		string FormatRichText(string richText, List<int> tabSizes);
		int ConvertToGraphicalCaretColumn(int logicalCaretColumn, ITextViewLine line);
		int ConvertToGraphicalCaretColumn(int logicalCaretColumn, ITextViewLine line, List<int> tabSizes);
		int ConvertToLogicalCaretColumn(int graphicalCaretColumn, ITextViewLine line);
		int ConvertToLogicalCaretColumn(int graphicalCaretColumn, ITextViewLine line, List<int> tabSizes);
	}
}
