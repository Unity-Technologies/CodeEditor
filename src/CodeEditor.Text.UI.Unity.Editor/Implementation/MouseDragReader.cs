using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	static class MouseDragReader
	{
		static Vector2 s_StartDragPosition = Vector2.zero;
		static Rect s_StartUserData;
		static Vector2 s_DraggedVector = Vector2.zero;

		public static Vector2 StartDragPosition
		{
			get { return s_StartDragPosition; }
		}
		public static Vector2 DraggedVector
		{
			get { return s_DraggedVector; }
		}
		public static Rect StartUserData
		{
			get { return s_StartUserData; }
		}

		// Get mouse delta values in different situations when click-dragging 
		public static bool DoDrag(int controlID, Rect dragRegion, bool activated, Rect startDragUserData)
		{
			Event evt = Event.current;
			switch (evt.GetTypeForControl(controlID))
			{
				case EventType.MouseDown:
					if (activated && GUIUtility.hotControl == 0 && dragRegion.Contains(evt.mousePosition) && evt.button == 0)
					{
						GUIUtility.hotControl = controlID;
						GUIUtility.keyboardControl = 0;
						s_StartDragPosition = GUIUtility.GUIToScreenPoint(evt.mousePosition); // GUIToScreenPoint to prevent being affected by scrollviews
						s_StartUserData = startDragUserData;
						evt.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID)
					{
						evt.Use();
						Vector2 screenPos = GUIUtility.GUIToScreenPoint(evt.mousePosition); // GUIToScreenPoint to prevent being affected by scrollviews
						s_DraggedVector = screenPos - s_StartDragPosition;
						return true;
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID && evt.button == 0)
					{
						GUIUtility.hotControl = 0;
						evt.Use();
					}
					break;
			}

			return false;
		}
	}
}
