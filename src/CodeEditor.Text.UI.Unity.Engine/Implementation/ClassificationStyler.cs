using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Text.Logic;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(IClassificationStyler))]
	public class ClassificationStyler : IClassificationStyler
	{
		private readonly IStandardClassificationRegistry _standardClassificationRegistry;
		private Dictionary<IClassification, Color> _classificationColors;

		[ImportingConstructor]
		public ClassificationStyler(IStandardClassificationRegistry standardClassificationRegistry)
		{
			_standardClassificationRegistry = standardClassificationRegistry;

			// Default colors (for a dark background)
			_classificationColors = new Dictionary<IClassification, Color>
			{
				{_standardClassificationRegistry.Keyword, Colors.LightBlue},
				{_standardClassificationRegistry.Identifier, Colors.White},
				{_standardClassificationRegistry.WhiteSpace, Colors.DarkYellow},
				{_standardClassificationRegistry.Text, Colors.White},
				{_standardClassificationRegistry.Operator, Colors.Pink},
				{_standardClassificationRegistry.Punctuation, Colors.Offwhite},
				{_standardClassificationRegistry.String, Colors.LightBrown},
				{_standardClassificationRegistry.Comment, Colors.Grey},
				{_standardClassificationRegistry.Number, Colors.Green},
			};
		}

		public event EventHandler Changed;

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		public IStandardClassificationRegistry StandardClassificationRegistry
		{
			get { return _standardClassificationRegistry; }
		}

		public Dictionary<IClassification, Color> ClassificationColors 
		{ 
			get { return _classificationColors; }
			set { _classificationColors = value; OnChanged(); }
		}

		public Color ColorFor(IClassification classification)
		{
			return _classificationColors[classification];
		}
	}
}
