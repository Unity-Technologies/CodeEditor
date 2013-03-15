using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IMouseCursorRegions))]
	[Export(typeof(IMouseCursors))]
	class UnityEditorMouseCursors: IMouseCursorRegions, IMouseCursors
	{
		public IMouseCursor Text { get; private set; }
		public IMouseCursor Finger { get; private set; }

		public UnityEditorMouseCursors()
		{
			Text = new UnityEditorMouseCursor(UnityEditor.MouseCursor.Text);
			Finger = new UnityEditorMouseCursor(UnityEditor.MouseCursor.Orbit);// UnityEditor api lacks a finger mouse cursor
		}
	
		public void AddMouseCursorRegion(Rect region, IMouseCursor cursor)
		{
			UnityEditor.EditorGUIUtility.AddCursorRect(region, MouseCursorFor(cursor));
		}

		UnityEditor.MouseCursor MouseCursorFor(IMouseCursor cursor)
		{
			return ((UnityEditorMouseCursor)cursor).MouseCursor;
		}

		class UnityEditorMouseCursor : IMouseCursor
		{
			UnityEditor.MouseCursor _unityMouseCursor;
			public UnityEditor.MouseCursor MouseCursor
			{
				get { return _unityMouseCursor; }
			}

			public UnityEditorMouseCursor(UnityEditor.MouseCursor cursor)
			{
				_unityMouseCursor = cursor;
			}
		}
	}
}
