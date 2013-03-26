using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEditor.Text.UI.Unity.Engine
{
	/// <summary>
	/// Must be implemented by the <see cref="ITextView"/> host.
	/// </summary>
	public interface IFontManagerProvider
	{
		IFontManager GetFontManager(ISettings settings);
	}
}
