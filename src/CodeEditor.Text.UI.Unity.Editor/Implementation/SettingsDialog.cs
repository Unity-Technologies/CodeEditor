using System.Collections.Generic;
using System.Linq;
using CodeEditor.Text.Logic;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class SettingsDialog
	{
		Rect _settingsWindowRect = new Rect(0, 0, 200, 20);
		readonly IntSetting _colorSchemeIndex;
		readonly int[] _fontSizes;
		readonly GUIContent[] _fontSizesNames;

		readonly IMouseCursorRegions _mouseCursorsRegions;
		readonly IMouseCursors _mouseCursors;
		readonly ISettings _settings;
		readonly IFontManager _fontManager;
		readonly IClassificationStyler _classificationStyler;
		readonly ITextViewAppearance _appearance;

		public SettingsDialog(ITextView textView)
		{
			_settings = textView.Settings;
			_mouseCursorsRegions = textView.MouseCursorsRegions;
			_mouseCursors = textView.MouseCursors;
			_fontManager = textView.FontManager;
			_classificationStyler = textView.Document.ClassificationStyler;
			_appearance = textView.Appearance;

			_colorSchemeIndex = new IntSetting("ColorSchemeIndex", 0, _settings);
			_colorSchemeIndex.Changed += (sender, args) => SetClassificationColors();

			_fontSizes = _fontManager.GetCurrentFontSizes();
			_fontSizesNames = _fontSizes.Select(size => new GUIContent(size.ToString())).ToArray();
			SetClassificationColors();
		}

		public void OnGUI(Rect availableRect)
		{
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

			var lineNumberMarginVisiblity = _settings.GetSetting("LineNumberVisiblitySetting") as BoolSetting;
			if (lineNumberMarginVisiblity != null)
				lineNumberMarginVisiblity.Value = EditorGUILayout.Toggle(new GUIContent("Line Numbers"),
					lineNumberMarginVisiblity.Value);

			var whitespaceVisibility = _settings.GetSetting("VisibleWhitespace") as BoolSetting;
			if (whitespaceVisibility != null)
				whitespaceVisibility.Value = EditorGUILayout.Toggle(new GUIContent("Whitespace"), whitespaceVisibility.Value);

			GUILayout.Label("Tab", EditorStyles.boldLabel);

			var numSpacePerTab = _settings.GetSetting("NumSpacesPerTab") as IntSetting;
			if (numSpacePerTab != null)
				numSpacePerTab.Value = EditorGUILayout.IntField(new GUIContent("Tab Size"), numSpacePerTab.Value);

			GUILayout.Label("Font", EditorStyles.boldLabel);

			_fontManager.CurrentFontSize = EditorGUILayout.IntPopup(new GUIContent("Font Size"), _fontManager.CurrentFontSize,
				_fontSizesNames, _fontSizes);

			GUILayout.Label("Colors", EditorStyles.boldLabel);

			_colorSchemeIndex.Value = EditorGUILayout.IntPopup(new GUIContent("Color scheme"), _colorSchemeIndex.Value,
				new[] {new GUIContent("Dark"), new GUIContent("Light")}, new[] {0, 1});

			GUILayout.Label("Code Completion", EditorStyles.boldLabel);
			var completionEnabled = _settings.GetSetting("CompletionEnabled") as BoolSetting;
			if (completionEnabled != null)
				completionEnabled.Value = EditorGUILayout.Toggle(new GUIContent("Enabled"), completionEnabled.Value);

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
			var bgDark1 = new Color(0 / 255f, 43 / 255f, 54 / 255f);
			var bgDark2 = new Color(51 / 255f, 51 / 255f, 51 / 255f);
			//Color bgLight1 = new Color (238/255f, 232/255f, 213/255f);
			var bgLight2 = new Color(253 / 255f, 246 / 255f, 227 / 255f);
			var grey = new Color(130 / 255f, 148 / 255f, 150 / 255f);
			var yellow = new Color(181 / 255f, 137 / 255f, 0 / 255f);
			var orange = new Color(204 / 255f, 75 / 255f, 22 / 255f);
			var red = new Color(220 / 255f, 50 / 255f, 47 / 255f);
			var magenta = new Color(211 / 255f, 54 / 255f, 130 / 255f);
			//Color violet = new Color(108/255f, 113/255f, 196/255f);
			var blue = new Color(28 / 255f, 129 / 255f, 200 / 255f);
			var unityProSelectionBlue = new Color(61 / 255f, 96 / 255f, 145 / 255f);
			var unitySelectionBlue = new Color(61 / 255f, 128 / 255f, 223 / 255f);
			var cyan = new Color(42 / 255f, 161 / 255f, 152 / 255f);
			var green = new Color(133 / 255f, 153 / 255f, 0 / 255f);

			Dictionary<IClassification, Color> syntaxColors;
			Color selectionColor;
			Color lineNumberColor;
			Color backgroundColor;

			if (_colorSchemeIndex.Value == 0)
			{
				// Dark background
				backgroundColor = bgDark2;
				selectionColor = unityProSelectionBlue;
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
