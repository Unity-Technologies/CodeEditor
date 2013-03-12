namespace CodeEditor.Text.UI
{
	/// <summary>
	/// Provides a way to open files at specific locations denoted by anchors.
	/// 
	/// Host interface.
	/// </summary>
	public interface IFileNavigationService
	{
		/// <summary>
		/// Opens the specified file at the optional location denoted by <paramref name="anchor"/>.
		/// </summary>
		void NavigateTo(string fileName, IAnchor anchor = null);
	}

	/// <summary>
	/// Represents a position in a document.
	/// 
	/// The actual semantics depend on the type of anchor.
	/// </summary>
	public interface IAnchor
	{
	}

	/// <summary>
	/// An anchor to a specific line and column of a text file.
	/// </summary>
	public class PositionAnchor : IAnchor
	{
		public PositionAnchor(int line, int column)
		{
			Position = new Position(line, column);
		}

		public Position Position { get; private set; }
	}
}
