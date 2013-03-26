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
		public ITextViewAppearance AppearanceFor(ITextViewDocument document, IFontManager fontManager)
		{
			return new TextViewAppearance(fontManager);
		}
	}

	public class TextViewAppearance : ITextViewAppearance
	{
		private readonly GUIStyle _background;
		private readonly GUIStyle _text;
		private readonly GUIStyle _lineNumber;
		private readonly Color _lineNumberColor;
		private readonly Color _selectionColor;
		private readonly IFontManager _fontManager;

		public event EventHandler Changed;

		public TextViewAppearance(IFontManager fontManager)
		{
			_fontManager = fontManager;
			_fontManager.Changed += (Sender, Args) => OnFontChanged();

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
