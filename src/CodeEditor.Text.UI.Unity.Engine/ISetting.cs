using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ISetting
	{
		string ID { get; set; }
		event EventHandler Changed;
	}
}
