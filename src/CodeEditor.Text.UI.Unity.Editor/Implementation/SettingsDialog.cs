using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Serializable]
	public class SettingsDialog
	{
		[SerializeField] 
		Rect _settingsWindowRect = new Rect (0,0, 200, 20);

		[NonSerialized]
		int[] _fontSizes = null;
		[NonSerialized]
		GUIContent[] _fontSizesNames;

		readonly IMouseCursorRegions _mouseCursorsRegions;
		readonly IMouseCursors _mouseCursors;
		readonly ISettings _settings;
		readonly IFontManager _fontManager;

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

			_fontSizes = _fontManager.GetCurrentFontSizes();
			var names = new List<GUIContent>();
			foreach (int size in _fontSizes)
				names.Add(new GUIContent(size.ToString()));
			_fontSizesNames = names.ToArray();
		}

		void InitIfNeeded()
		{
			if (s_Styles == null)
				s_Styles = new Styles();
		}

		public void OnGUI(Rect availableRect)
		{
			InitIfNeeded();

			_settingsWindowRect.x = availableRect.xMax - _settingsWindowRect.width - 14;
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
	}
}
