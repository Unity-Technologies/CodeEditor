namespace CodeEditor.IO
{
	public interface IFileSystem
	{
		IFile FileFor(string file);
	}
}
