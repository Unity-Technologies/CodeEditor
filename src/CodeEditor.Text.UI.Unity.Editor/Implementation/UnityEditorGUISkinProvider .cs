using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IGUISkinProvider))]
	class UnityEditorGUISkinProvider : IGUISkinProvider
	{
		GUISkin _guiSkin;

		public GUISkin GetGUISkin()
		{
			if (_guiSkin == null)
			{
				string userSkinPath = "Assets/Editor/CodeEditor/CodeEditorSkin.guiskin";
				_guiSkin = UnityEditor.AssetDatabase.LoadAssetAtPath(userSkinPath, typeof(GUISkin)) as GUISkin;
				if (_guiSkin == null)
				{
					Debug.Log("CodeEditor skin not found! " + userSkinPath);
					_guiSkin = GUI.skin;
				}
			}
			return _guiSkin;
		}
	}
}
