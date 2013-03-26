using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine
{
	/// <summary>
	/// Provides a way to assign mouse cursors to specific regions.
	/// </summary>
	public interface IMouseCursorRegions
	{
		void AddMouseCursorRegion(Rect region, IMouseCursor cursor);
	}

	/// <summary>
	/// Standard set of mouse cursors provided by the host.
	/// </summary>
	public interface IMouseCursors
	{
		IMouseCursor Arrow { get; }
		IMouseCursor Text { get; }
		IMouseCursor Finger { get; }
	}

	/// <summary>
	/// Represents a mouse cursor that can be rendered by the host.
	/// </summary>
	public interface IMouseCursor
	{
	}
}
