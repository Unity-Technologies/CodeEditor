using System;
using System.Collections.Generic;
using CodeEditor.Text.Logic;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public interface IClassificationStyler
	{
		Color ColorFor(IClassification classification);
		IStandardClassificationRegistry StandardClassificationRegistry { get; }
		Dictionary<IClassification, Color> ClassificationColors {get; set;}
		event EventHandler Changed;
	}
}
