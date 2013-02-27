using System.Diagnostics;
using System.IO;
using CodeEditor.Composition;

namespace CodeEditor.IO
{
	public interface IShell
	{
		IProcess StartManagedProcess(string executable);
	}

	public interface IProcess
	{
		int Id { get; }
		StreamReader StandardOutput { get; }
		StreamWriter StandardInput { get; }
		bool WaitForExit(int timeout);
	}

	[Export(typeof(IShell))]
	public class StandardShell : IShell
	{
		public IProcess StartManagedProcess(string executable)
		{
			return new StandardProcess(Process.Start(new ProcessStartInfo(executable)
			{
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			}));
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

			public StreamReader StandardOutput
			{
				get { return _process.StandardOutput; }
			}

			public StreamWriter StandardInput
			{
				get { return _process.StandardInput; }
			}

			public bool WaitForExit(int timeout)
			{
				return _process.WaitForExit(timeout);
			}
		}
	}
}