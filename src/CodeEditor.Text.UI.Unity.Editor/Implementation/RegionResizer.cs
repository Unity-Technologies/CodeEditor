using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	class RegionResizer
	{
		readonly float _borderWidth;
		BorderLocation[] _borderLocations;
		
		static bool s_Changed;
		static Vector2[] s_BorderDirections =
			{
				new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1),
				new Vector2(-1, 0), new Vector2(1, 0),
				new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1)
			};

		public enum BorderLocation
		{
			TopLeft, Top, TopRight,
			MiddleLeft, MiddleRight,
			BottomLeft, Bottom, BottomRight
		}
		
		public RegionResizer(float resizeBorderWidth, BorderLocation[] activeBorderLocations)
		{
			_borderWidth = resizeBorderWidth;
			_borderLocations = activeBorderLocations;
		}

		public bool HandleResizing (Rect region, Vector2 minSize, Vector2 maxSize, out Rect newRegion)
		{
			s_Changed = false;
			foreach (BorderLocation border in _borderLocations)
				region = ResizeBorderHandling(region, border, GetBorderDiretion(border), _borderWidth, minSize, maxSize);
			newRegion = region;
			return s_Changed;
		}

		static Rect ResizeBorderHandling(Rect region, BorderLocation borderLocation, Vector2 direction, float borderWidth, Vector2 minSize, Vector2 maxSize)
		{
			Rect dragRegion = GetBorderRect(region, borderLocation, borderWidth);
			int controlID = GUIUtility.GetControlID(9197383, FocusType.Passive);

			if (MouseDragReader.DoDrag(controlID, dragRegion, true, region))
			{
				Vector2 offset = MouseDragReader.DraggedVector;
				offset.x *= direction.x;
				offset.y *= direction.y;
				Vector2 orgSize = new Vector2(MouseDragReader.StartUserData.width, MouseDragReader.StartUserData.height);
				Vector2 orgPos = new Vector2(MouseDragReader.StartUserData.x, MouseDragReader.StartUserData.y);
				Vector2 newSize = offset + orgSize;
				float newWidth = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
				float newHeight = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);

				float newX = orgPos.x;
				if (direction.x < -0.5f)
					newX -= (newWidth - orgSize.x);

				float newY = orgPos.y;
				if (direction.y < -0.5f)
					newY -= (newHeight - orgSize.y);

				region = new Rect(newX, newY, newWidth, newHeight);
				s_Changed = true;
			}

			EditorGUIUtility.AddCursorRect(dragRegion, GetIconForDirection(direction), controlID);

			return region;
		}

		static Vector2 GetBorderDiretion(BorderLocation borderLocation)
		{
			return s_BorderDirections[(int)borderLocation];
		}

		static Rect GetBorderRect(Rect region, BorderLocation border, float borderWidth)
		{
			switch (border)
			{
				case BorderLocation.TopLeft: return new Rect(0, 0, borderWidth, borderWidth);
				case BorderLocation.Top: return new Rect(borderWidth, 0, region.width - 2 * borderWidth, borderWidth);
				case BorderLocation.TopRight: return new Rect(region.width - 2 * borderWidth, 0, borderWidth, borderWidth);
				case BorderLocation.MiddleLeft: return new Rect(0, borderWidth, borderWidth, region.height - 2 * borderWidth);
				case BorderLocation.MiddleRight: return new Rect(region.width - borderWidth, borderWidth, borderWidth, region.height - 2 * borderWidth);
				case BorderLocation.BottomLeft: return new Rect(0, region.height - borderWidth, borderWidth, borderWidth);
				case BorderLocation.Bottom: return new Rect(borderWidth, region.height - borderWidth, region.width - 2 * borderWidth, borderWidth);
				case BorderLocation.BottomRight: return new Rect(region.width - borderWidth, region.height - borderWidth, borderWidth, borderWidth);
				default:
					return new Rect();
			}
		}

		static MouseCursor GetIconForDirection(Vector2 direction)
		{
			if (direction.x != 0f && direction.y != 0f)
				return direction.x * direction.y < 0 ? MouseCursor.ResizeUpRight : MouseCursor.ResizeUpLeft;

			if (direction.x != 0f)
				return MouseCursor.ResizeHorizontal;

			return MouseCursor.ResizeVertical;
		}
	}
}
