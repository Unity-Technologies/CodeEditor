using CodeEditor.Text.Logic;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface IClassificationStyler
	{
		Color ColorFor(IClassification classification);
	}
}
