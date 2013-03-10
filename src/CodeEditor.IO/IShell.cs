using System;
using System.Diagnostics;
using CodeEditor.Composition;

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

	[Export(typeof(IShell))]
	public class StandardShell : IShell
	{
		public IProcess StartManagedProcess(string executable)
		{
			return new StandardProcess(Process.Start(new ProcessStartInfo(MonoExecutable, executable)
			{
				WindowStyle = ProcessWindowStyle.Minimized
			}));
		}

		static string MonoExecutable
		{
			get { return Environment.GetEnvironmentVariable("MONO_EXECUTABLE") ?? "mono"; }
		}

		public class StandardProcess : IProcess
		{
			private readonly Process _process;
			
			public StandardProcess(Process process)
			{
				_process = process;
			}

			public int Id
			{
				get { return _process.Id; }
			}

			public bool WaitForExit(int timeout)
			{
				return _process.WaitForExit(timeout);
			}

			public void Kill()
			{
				_process.Kill();
			}

			public void Dispose()
			{
				_process.Dispose();
			}
		}
	}
}