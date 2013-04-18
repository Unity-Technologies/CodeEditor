using System;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface ISetting
	{
		string ID { get; set; }
		event EventHandler Changed;
	}
}
