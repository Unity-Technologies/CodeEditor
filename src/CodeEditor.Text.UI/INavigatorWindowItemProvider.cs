using System.Collections.Generic;

namespace CodeEditor.Text.UI
{
	public interface INavigatorWindowItemProvider
	{
		List<INavigatorWindowItem> Search(string filter);
	}
}
