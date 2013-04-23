using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public interface IIcons
	{
		Texture2D GetIcon(string iconNameWithExtension);
	}
}
