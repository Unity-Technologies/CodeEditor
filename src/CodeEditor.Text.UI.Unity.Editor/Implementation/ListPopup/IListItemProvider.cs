using System.Collections.Generic;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	public interface IListItemProvider
	{
		List<IListItem> GetList();
	}
}
