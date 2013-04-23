using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IIcons))]
	class Icons : IIcons
	{
		Dictionary<string, Texture2D> _loadedIcons = new Dictionary<string, Texture2D>();
		List<string> _failedIconSearches = new List<string>(); 
		public string CodeEditorFolderPath { get; set; }
		public string IconFolder { get; set; }

		public Icons()
		{
			CodeEditorFolderPath = "Assets/Editor/CodeEditor/";
			IconFolder = "Textures/Icons/";
		}

		public Texture2D GetIcon(string iconNameWithExtension)
		{
			Texture2D icon;
			if (_loadedIcons.TryGetValue(iconNameWithExtension, out icon))
			{
				return icon;
			}
			else
			{
				icon = AssetDatabase.LoadAssetAtPath(GetFullIconPath(iconNameWithExtension), typeof(Texture2D)) as Texture2D;
				if (icon != null)
				{
					_loadedIcons[iconNameWithExtension] = icon;
				}
				else
				{
					if (!_failedIconSearches.Contains(iconNameWithExtension))
					{
						_failedIconSearches.Add(iconNameWithExtension);
						Debug.LogError("Icon '" + iconNameWithExtension + "' not found at: " + 
							GetFullIconPath(iconNameWithExtension) + ". Did you remember the file extension?");
					}
				}
				return icon;
			}
		}

		public string[] GetIconFilePaths()
		{
			return System.IO.Directory.GetFiles( System.IO.Path.Combine(CodeEditorFolderPath, IconFolder), "*.png");
		}

		string GetFullIconPath(string iconNameWithExtension)
		{
			return CodeEditorFolderPath + IconFolder + iconNameWithExtension;
		}
	}
}
