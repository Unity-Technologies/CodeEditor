namespace CodeEditor.IO
{
	public interface IFile
	{
		string Extension { get; }
		string ReadAllText();
		void WriteAllText(string text);
	}
}
