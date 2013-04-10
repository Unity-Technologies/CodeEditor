using System;

namespace CodeEditor.IO
{
	public interface IShell
	{
		IProcess StartManagedProcess(string executable);
	}

	public interface IProcess : IDisposable
	{
		int Id { get; }
		bool WaitForExit(int timeout);
		void Kill();
	}

	public interface IMonoExecutableProvider
	{
		string MonoExecutable { get; }
	}
}