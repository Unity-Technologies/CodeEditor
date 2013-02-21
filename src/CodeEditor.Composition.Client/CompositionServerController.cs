using System;
using System.Diagnostics;
using System.IO;

namespace CodeEditor.Composition.Client
{
	public class CompositionServerController : IDisposable
	{
		public static CompositionServerController StartCompositionServerAt(string serverFolder)
		{
			var compositionServerExe = Path.Combine(serverFolder, "CodeEditor.Composition.Server.exe");
			var serverProcess = StartServerProcess(compositionServerExe);
			serverProcess.StandardOutput.ReadLine(); // wait for the server to be up
			return new CompositionServerController(serverProcess);
		}

		private static Process StartServerProcess(string compositionServerExe)
		{
			return Process.Start(new ProcessStartInfo(compositionServerExe)
			{
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			});
		}

		private readonly Process _serverProcess;

		private CompositionServerController(Process serverProcess)
		{
			_serverProcess = serverProcess;
		}

		public int Id
		{
			get { return _serverProcess.Id; }
		}

		public bool Stop(int timeout)
		{
			_serverProcess.StandardInput.WriteLine("quit");
			return _serverProcess.WaitForExit(timeout);
		}

		public void Dispose()
		{
			if (!Stop(2000))
				throw new InvalidOperationException("Composition server failed to quit in the alloted time.");
		}
	}
}