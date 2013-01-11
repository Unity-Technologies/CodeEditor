using System.Collections.Generic;

namespace CodeEditor.Text.UI
{
	public interface INavigateToItemProvider
	{
		List<INavigateToItem> Search(string filter);
	}
}
