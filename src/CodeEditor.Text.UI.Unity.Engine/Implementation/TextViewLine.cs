using CodeEditor.Text.Data;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	internal class TextViewLine : ITextViewLine
	{
		private readonly TextViewDocument _owner;
		private readonly ITextSnapshotLine _textLine;
		private string _richText;

		public TextViewLine(TextViewDocument owner, ITextSnapshotLine textLine)
		{
			_textLine = textLine;
			_owner = owner;
		}

		public string Text
		{
			get { return _textLine.Text; }
		}

		public string RichText
		{
			get { return _richText ?? (_richText = CreateRichText()); }
		}

		public int Start
		{
			get { return _textLine.Start; }
		}

		public int LineNumber
		{
			get { return _textLine.LineNumber; }
		}

		string CreateRichText()
		{
			var textBuilder = new System.Text.StringBuilder();
			foreach (var span in _owner.Classify(_textLine))
			{
				textBuilder.Append("<color=#");
				textBuilder.Append(HexifyColor(_owner.ColorFor(span.Classification)));
				textBuilder.Append(">");
				textBuilder.Append(Text, span.Start - _textLine.Start, span.Length);
				textBuilder.Append("</color>");
			}
			return textBuilder.ToString();
		}

		private static string HexifyColor(Color color)
		{
			return HexifyColorComponent(color.r) + HexifyColorComponent(color.g) + HexifyColorComponent(color.b);
		}

		private static string HexifyColorComponent(float c)
		{
			return Mathf.FloorToInt(c * 255).ToString("x2");
		}
	}
}
