using System.Collections.Generic;
using System.Diagnostics;
using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IShell))]
	public class StandardShell : IShell
	{
		[Import]
		public IMonoExecutableProvider MonoExecutableProvider { get; set; }

		public IProcess StartManagedProcess(ProcessSettings settings)
		{
			return new StandardProcess(Process.Start(StartInfoFrom(settings)));
		}

		ProcessStartInfo StartInfoFrom(ProcessSettings settings)
		{
			var startInfo = new ProcessStartInfo(MonoExecutable, settings.Executable);
			startInfo.WorkingDirectory = settings.Executable.Parent;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.UseShellExecute = false;
			foreach (var variable in settings.EnvironmentVariables)
				startInfo.EnvironmentVariables.Add(variable.Key, variable.Value);
			return startInfo;
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