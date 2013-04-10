using System.Diagnostics;
using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IShell))]
	public class StandardShell : IShell
	{
		[Import]
		public IMonoExecutableProvider MonoExecutableProvider { get; set; }

		public IProcess StartManagedProcess(string executable)
		{
			return new StandardProcess(Process.Start(new ProcessStartInfo(MonoExecutable, executable)
			{
				WindowStyle = ProcessWindowStyle.Minimized
			}));
		}

		string MonoExecutable
		{
			get { return MonoExecutableProvider.MonoExecutable; }
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