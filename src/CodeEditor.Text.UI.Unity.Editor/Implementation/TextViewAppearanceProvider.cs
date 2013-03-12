using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(ITextViewAppearanceProvider))]
	class TextViewAppearanceProvider : ITextViewAppearanceProvider
	{
		public ITextViewAppearance AppearanceFor(ITextViewDocument document)
		{
			return new TextViewAppearance();
		}
	}

	public class TextViewAppearance : ITextViewAppearance
	{
		private readonly GUIStyle _background;
		private readonly GUIStyle _text;
		private readonly GUIStyle _lineNumber;
		private readonly Color _lineNumberColor;
		private readonly Color _selectionColor;
		private Font[] _availableFonts;
		private int[] _availableFontSizes;

		public event EventHandler Changed;

		public TextViewAppearance()
		{
			FindAvailableFontSizes();

			_background = "AnimationCurveEditorBackground";
			string userSkinPath = "Assets/Editor/CodeEditor/CodeEditorSkin.guiskin";

			GUISkin skin = UnityEditor.AssetDatabase.LoadAssetAtPath(userSkinPath, typeof(GUISkin)) as GUISkin;
			if (skin == null)
			{
				//Debug.Log ("Could not find user skin at: " + userSkinPath + ". Using default skin");
				skin = GUI.skin;
			}
			else
			{
				//Debug.Log ("User skin found, font: " + skin.font.name);
			}

			_text = new GUIStyle(skin.label)
			{
				richText = true,
				alignment = TextAnchor.UpperLeft,
				padding = { left = 0, right = 0 }
			};
			_text.normal.textColor = Color.white;

			_lineNumber = new GUIStyle (_text)
			{
				richText = false,
				alignment = TextAnchor.UpperRight
			};

			_lineNumberColor = new Color(1, 1, 1, 0.5f);

			_selectionColor = new Color(80/255f, 80/255f, 80/255f, 1f);
		}

		void FindAvailableFontSizes()
		{
			string fontBasePath = "Assets/Editor/CodeEditor/Fonts/"; // TODO make this not so hardcoded...
			string fontName = "SourceCodePro-Regular";
			List<Font> fonts = new List<Font>();
			List<int> fontSizes = new List<int>();

			// Search for font sizes in the following range:
			const int minFontSize = 6;
			const int maxFontSize = 40;
			for (int i = minFontSize; i <= maxFontSize; ++i)
			{
				string fontPath = System.IO.Path.Combine(fontBasePath, fontName + i + ".ttf");
				Font font = UnityEditor.AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;
				if (font != null)
				{
					fonts.Add(font);
					fontSizes.Add(i);
				}
			}
			_availableFonts = fonts.ToArray();
			_availableFontSizes = fontSizes.ToArray();
		}

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		public int[] GetSupportedFontSizes()
		{
			return _availableFontSizes;
		}

		public void SetFontSize(int fontSize)
		{
			int index = Array.IndexOf(_availableFontSizes, fontSize);
			if(index >= 0)
			{
				_text.font = _availableFonts[index];
				_lineNumber.font = _text.font;
				OnChanged();
			}
		}

		public GUIStyle Background
		{
			get { return _background; }
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
		}

		public Color SelectionColor
		{
			get { return _selectionColor; }
		}
	}
}
