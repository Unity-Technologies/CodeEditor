using System.Collections.Generic;
using CodeEditor.Text.Logic;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class SettingsDialog
	{
		Rect _settingsWindowRect = new Rect (0,0, 200, 20);
		IntSetting _colorSchemeIndex;
		int[] _fontSizes = null;
		GUIContent[] _fontSizesNames;

		readonly IMouseCursorRegions _mouseCursorsRegions;
		readonly IMouseCursors _mouseCursors;
		readonly ISettings _settings;
		readonly IFontManager _fontManager;
		readonly IClassificationStyler _classificationStyler;
		readonly ITextViewAppearance _appearance;

		class Styles
		{
			public GUIStyle rightAlignedLabel = new GUIStyle(GUI.skin.label);
			public Styles()
			{ 
				rightAlignedLabel.alignment = TextAnchor.MiddleRight;
				rightAlignedLabel.fontSize = 12;
			}
		}
		static Styles s_Styles;


		public SettingsDialog(ITextView textView)
		{
			_settings = textView.Settings;
			_mouseCursorsRegions = textView.MouseCursorsRegions;
			_mouseCursors = textView.MouseCursors;
			_fontManager = textView.FontManager;
			_classificationStyler = textView.Document.ClassificationStyler;
			_appearance = textView.Appearance;

			_colorSchemeIndex = new IntSetting("ColorSchemeIndex", 0, _settings);
			_colorSchemeIndex.Changed += (Sender, Args) => SetClassificationColors();

			_fontSizes = _fontManager.GetCurrentFontSizes();
			var names = new List<GUIContent>();
			foreach (int size in _fontSizes)
				names.Add(new GUIContent(size.ToString()));
			_fontSizesNames = names.ToArray();

			SetClassificationColors();
		}

		void InitIfNeeded()
		{
			if (s_Styles == null)
				s_Styles = new Styles();
		}

		public void OnGUI(Rect availableRect)
		{
			InitIfNeeded();

			_settingsWindowRect.x = availableRect.xMax - _settingsWindowRect.width - 17;
			_settingsWindowRect.y = availableRect.y + 1;

			_mouseCursorsRegions.AddMouseCursorRegion(_settingsWindowRect, _mouseCursors.Arrow);
			_settingsWindowRect = GUI.Window(0, _settingsWindowRect, DoWindow, new GUIContent("Options"));
		}

		void DoWindow(int id)
		{
			const float topMargin = 0;

			Rect size = EditorGUILayout.BeginVertical();
			GUILayout.Space(topMargin);

			GUILayout.Label("View", EditorStyles.boldLabel);
			
			BoolSetting lineNumberMarginVisiblity = _settings.GetSetting("LineNumberVisiblitySetting") as BoolSetting;
			if (lineNumberMarginVisiblity != null)
				lineNumberMarginVisiblity.Value = EditorGUILayout.Toggle(new GUIContent("Line Numbers"), lineNumberMarginVisiblity.Value);
			
			BoolSetting whitespaceVisibility = _settings.GetSetting("VisibleWhitespace") as BoolSetting;
			if (whitespaceVisibility != null)
				whitespaceVisibility.Value = EditorGUILayout.Toggle(new GUIContent("Whitespace"), whitespaceVisibility.Value);

			GUILayout.Label("Tab", EditorStyles.boldLabel);
			
			IntSetting numSpacePerTab = _settings.GetSetting("NumSpacesPerTab") as IntSetting;
			if (numSpacePerTab != null)
				numSpacePerTab.Value = EditorGUILayout.IntField(new GUIContent("Tab Size"), numSpacePerTab.Value);

			GUILayout.Label("Font", EditorStyles.boldLabel);
			
			_fontManager.CurrentFontSize = EditorGUILayout.IntPopup(new GUIContent("Font Size"), _fontManager.CurrentFontSize, _fontSizesNames, _fontSizes);

			GUILayout.Label("Colors", EditorStyles.boldLabel);

			_colorSchemeIndex.Value = EditorGUILayout.IntPopup(new GUIContent("Color scheme"), _colorSchemeIndex.Value, new []{new GUIContent("Dark"), new GUIContent("Light")}, new []{0,1});

			GUILayout.Space(10);

			// Make window draggable
			//GUI.DragWindow(new Rect(0, 0, 10000, 10000));

			// Use event if clicking inside window
			Event evt = Event.current;
			if (evt.type == EventType.MouseDown && _settingsWindowRect.Contains(Event.current.mousePosition))
				Event.current.Use();

			GUILayout.EndVertical();

			if (size.height > 30) 
				_settingsWindowRect.height = size.height + 20f; // + window header
		}

		void SetClassificationColors()
		{
			Color bgDark1 = new Color (0/255f, 43/255f, 54/255f);
			Color bgDark2 = new Color(37 / 255f, 39 / 255f, 39 / 255f);
			Color bgLight1 = new Color (238/255f, 232/255f, 213/255f);
			Color bgLight2 = new Color (253/255f, 246/255f, 227/255f);
			Color grey = new Color(130 / 255f, 148 / 255f, 150 / 255f);
			Color yellow = new Color (181/255f, 137/255f, 0/255f);
			Color orange = new Color (204/255f, 75/255f, 22/255f);
			Color red = new Color(220/255f, 50/255f, 47/255f);
			Color magenta = new Color(211/255f, 54/255f, 130/255f);
			Color violet = new Color(108/255f, 113/255f, 196/255f);
			Color blue = new Color(28/255f, 129/255f, 200/255f);
			Color cyan = new Color(42/255f, 161/255f, 152/255f);
			Color green = new Color(133/255f, 153/255f, 0/255f);

			Dictionary<IClassification, Color> syntaxColors;
			Color selectionColor;
			Color lineNumberColor;
			Color backgroundColor;

			if (_colorSchemeIndex.Value == 0)
			{
				// Dark background
				backgroundColor = bgDark2;
				selectionColor = grey;
				lineNumberColor = grey;
				syntaxColors = new Dictionary<IClassification, Color>
				{
					{StandardClassificationRegistry.Keyword, Colors.LightBlue},
					{StandardClassificationRegistry.Identifier, Colors.White},
					{StandardClassificationRegistry.WhiteSpace, Colors.DarkYellow},
					{StandardClassificationRegistry.Text, Colors.White},
					{StandardClassificationRegistry.Operator, Colors.Pink},
					{StandardClassificationRegistry.Punctuation, Colors.Offwhite},
					{StandardClassificationRegistry.String, Colors.LightBrown},
					{StandardClassificationRegistry.Comment, Colors.Grey},
					{StandardClassificationRegistry.Number, Colors.Green}
				};
			}
			else
			{
				// Light background
				backgroundColor = bgLight2;
				selectionColor = grey;
				lineNumberColor = grey;
				syntaxColors = new Dictionary<IClassification, Color>
				{
					{StandardClassificationRegistry.Keyword, blue},
					{StandardClassificationRegistry.Identifier, bgDark1},
					{StandardClassificationRegistry.WhiteSpace, grey},
					{StandardClassificationRegistry.Text, yellow},
					{StandardClassificationRegistry.Operator, magenta},
					{StandardClassificationRegistry.Punctuation, orange},
					{StandardClassificationRegistry.String, cyan},
					{StandardClassificationRegistry.Comment, green},
					{StandardClassificationRegistry.Number, red}
				};
			}
			
			_classificationStyler.ClassificationColors = syntaxColors;
			_appearance.LineNumberColor = lineNumberColor;
			_appearance.SelectionColor = selectionColor;
			_appearance.BackgroundColor = backgroundColor;
		}
		
		IStandardClassificationRegistry StandardClassificationRegistry
		{
			get { return _classificationStyler.StandardClassificationRegistry; }
		}
	}
}
