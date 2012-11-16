using System.Collections.Generic;

namespace CodeEditor.Text.UI
{
	public interface IFilePathProvider
	{
		List<FilePathProviderItem> GetItems(string filter);
		bool GetFileAndLineNumber(object userData, out string filePath, out int lineNumber);
	}
}

