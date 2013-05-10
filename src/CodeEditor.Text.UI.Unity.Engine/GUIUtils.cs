using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	public class GUIUtils
	{
		private static Texture2D _whiteTexture;

		public static void DrawRect(Rect rect, Color color)
		{
			var backup = GUI.color;
			GUI.color =  new Color(color.r, color.g, color.b);
			GUI.DrawTexture(rect, whiteTexture, ScaleMode.StretchToFill, false);
			GUI.color = backup;
		}

		private static Texture2D whiteTexture
		{
			get
			{
				if (_whiteTexture != null)
					return _whiteTexture;
				
				_whiteTexture = new Texture2D(1, 1);
				_whiteTexture.SetPixel(0, 0, Color.white);
				_whiteTexture.hideFlags = HideFlags.HideAndDontSave;
				return _whiteTexture;
			}
		}
	}
}
