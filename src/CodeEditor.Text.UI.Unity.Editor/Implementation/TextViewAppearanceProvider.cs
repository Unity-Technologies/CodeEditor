using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(ITextViewAppearanceProvider))]
	class TextViewAppearanceProvider : ITextViewAppearanceProvider
	{
		public ITextViewAppearance AppearanceFor(ITextViewDocument document, IFontManager fontManager)
		{
			string userSkinPath = "Assets/Editor/CodeEditor/CodeEditorSkin.guiskin";
			GUISkin skin = UnityEditor.AssetDatabase.LoadAssetAtPath(userSkinPath, typeof(GUISkin)) as GUISkin;
			if (skin == null)
				skin = GUI.skin;

			// Make a copy of guistyle to ensure we do not change the guistyle of the skin
			GUIStyle textStyle = new GUIStyle(skin.label)
			{
				richText = true,
				alignment = TextAnchor.UpperLeft,
				padding = { left = 0, right = 0 }
			};
			textStyle.normal.textColor = Color.white;

			GUIStyle backgroundStyle = skin.GetStyle("InnerShadowBg");
			Color lineNumberColor = new Color(1, 1, 1, 0.5f);
			Color selectionColor = new Color(80 / 255f, 80 / 255f, 80 / 255f, 1f);
			return new TextViewAppearance(fontManager, textStyle, backgroundStyle, lineNumberColor, selectionColor);
		}
	}

	public class TextViewAppearance : ITextViewAppearance
	{
		private GUIStyle _background;
		private GUIStyle _text;
		private GUIStyle _lineNumber;
		private Color _lineNumberColor;
		private Color _selectionColor;
		private Color _backgroundColor;
		private readonly IFontManager _fontManager;

		public event EventHandler Changed;

		public TextViewAppearance(IFontManager fontManager, GUIStyle textStyle, GUIStyle backgroundStyle, Color lineNumberColor, Color selectionColor)
		{
			_fontManager = fontManager;
			_fontManager.Changed += (Sender, Args) => OnFontChanged();
			_background = backgroundStyle;
			_text = textStyle;
			_lineNumberColor = lineNumberColor;
			_selectionColor = selectionColor;

			_lineNumber = new GUIStyle (_text)
			{
				richText = false,
				alignment = TextAnchor.UpperRight
			};

			_text.font = _lineNumber.font = _fontManager.CurrentFont;
		}

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		void OnFontChanged()
		{
			_text.font = _lineNumber.font = _fontManager.CurrentFont;
			OnChanged();
		}

		public GUIStyle Background
		{
			get { return _background; }
			set { _background = value; }
		}

		public GUIStyle Text
		{
			get { return _text; }
		}

		public GUIStyle LineNumber
		{
			get { return _lineNumber; }
		}

		public Color LineNumberColor
		{
			get { return _lineNumberColor; }
			set { _lineNumberColor = value; OnChanged(); }
		}

		public Color SelectionColor
		{
			get { return _selectionColor; }
			set { _selectionColor = value; OnChanged(); }
		}

		public Color BackgroundColor 
		{
			get { return _backgroundColor; }
			set { _backgroundColor = value; OnChanged(); }
		}
	}
}
