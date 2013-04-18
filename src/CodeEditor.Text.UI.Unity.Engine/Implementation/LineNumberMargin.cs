using CodeEditor.Text.Data;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	class LineNumberMargin : ITextViewMargin
	{
		private readonly ITextViewAppearance _appearance;
		private readonly ITextBuffer _buffer;
		private float _width;
		BoolSetting _visibilitySetting;

		public LineNumberMargin(ITextView textView, BoolSetting visibilitySetting)
		{
			_appearance = textView.Appearance;
			_appearance.Changed += (sender, args) => CalculateWidth();
			_buffer = textView.Document.Buffer;
			_buffer.Changed += (sender, args) => CalculateWidth();
			CalculateWidth();
			_visibilitySetting = visibilitySetting;
		}

		public bool Visible { get { return _visibilitySetting.Value;  } set {_visibilitySetting.Value = value;} }

		private void CalculateWidth()
		{
			var digits = LineCount > 99 ? LineCount.ToString() : "99";
			_width = LineNumberStyle.CalcSize(MissingEngineAPI.GUIContent_Temp(digits)).x;
		}

		public void Repaint(ITextViewLine line, Rect marginRect)
		{
			if (Event.current.type != EventType.repaint)
				return;

			var orgColor = GUI.color;
			GUI.color = _appearance.LineNumberColor;
			_appearance.LineNumber.Draw(marginRect, (line.LineNumber + 1).ToString(), false, false, false, false);
			GUI.color = orgColor;
		}

		public void HandleInputEvent(ITextViewLine line, Rect marginRect)
		{
		}

		private int LineCount
		{
			get { return _buffer.CurrentSnapshot.Lines.Count; }
		}

		private GUIStyle LineNumberStyle
		{
			get { return _appearance.LineNumber; }
		}

		public float Width
		{
			get { return _width; }
		}
	}
}
