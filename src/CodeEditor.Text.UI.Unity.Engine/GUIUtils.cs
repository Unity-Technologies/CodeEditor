using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public class GUIUtils
	{
		private static GUIStyle _style;

		public static void DrawRect(Rect rect, Color color)
		{
			var backup = GUI.color;
			GUI.color = color;
			GUI.Label(rect, GUIContent.none, Style);
			GUI.color = backup;
		}

		protected static GUIStyle Style
		{
			get
			{
				if (_style != null)
					return _style;

				_style = new GUIStyle();
				_style.normal.background = DummyTexture();
				_style.normal.textColor = Color.white;

				return _style;
			}
		}

		private static Texture2D DummyTexture()
		{
			var dummyTexture = new Texture2D(1, 1);
			dummyTexture.SetPixel(0,0,Color.white);
			dummyTexture.hideFlags = HideFlags.HideAndDontSave;
			return dummyTexture;
		}
	}
}
