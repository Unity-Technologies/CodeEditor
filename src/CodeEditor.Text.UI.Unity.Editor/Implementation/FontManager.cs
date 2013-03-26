using System;
using System.Collections.Generic;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	class FontManager : IFontManager
	{
		readonly StringSetting _currentFontName;
		readonly IntSetting _currentFontSize;
		Font[] _availableFonts;
		int[] _availableFontSizes;
		bool _initialized;

		public FontManager(StringSetting currentFontName, IntSetting currentFontSize)
		{
			_currentFontName = currentFontName;
			_currentFontSize = currentFontSize;
			InitIfNeeded();
		}

		void InitIfNeeded()
		{
			if (_initialized)
				return;
			
			InitAvailableFontSizesFor(CurrentFontName);
			EnsureValidFont();

			_initialized = true;
		}

		void EnsureValidFont()
		{
			if (GetCurrentFontSizes().Length == 0)
				return;

			if (Array.IndexOf(GetCurrentFontSizes(), CurrentFontSize) < 0)
				CurrentFontSize = GetCurrentFontSizes()[0];
		}

		public string[] GetSupportedFontNames()
		{
			return new [] {CurrentFontName};
		}

		public int[] GetCurrentFontSizes()
		{
			return _availableFontSizes;
		}

		public Font CurrentFont
		{
			get
			{
				int index = Array.IndexOf(_availableFontSizes, CurrentFontSize);
				if (index >= 0)
					return _availableFonts[index];

				return null;
			}
		}

		public string CurrentFontName
		{
			get { return _currentFontName.Value; }
			set {
				if (_currentFontName.Value != value)
				{
					_currentFontName.Value = value;
					OnChanged();
				}
			}
		}

		public int CurrentFontSize 
		{
			get { return _currentFontSize.Value; }
			set
			{
				if (_currentFontSize.Value != value)
				{
					_currentFontSize.Value = value;
					EnsureValidFont();
					OnChanged();
				}
			}
		}

		public event EventHandler Changed;

		protected void OnChanged()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		void InitAvailableFontSizesFor(string fontName)
		{
			string fontBasePath = "Assets/Editor/CodeEditor/Fonts/"; // TODO make this not so hardcoded...
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

			if (_availableFonts.Length == 0)
				Debug.LogError("Did not find any fonts in " + fontBasePath);
		}
	}
}
