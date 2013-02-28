using System;
using System.Diagnostics;
using System.IO;
using CodeEditor.IO;

namespace CodeEditor.Composition.Client
{
	public interface ICompositionServerControllerFactory
	{
		ICompositionServerController StartCompositionServerAtFolder(string serverFolder);
	}

	[Export(typeof(ICompositionServerControllerFactory))]
	public class CompositionServerControllerFactory : ICompositionServerControllerFactory
	{
		[Import]
		public IShell Shell;

		public ICompositionServerController StartCompositionServerAtFolder(string serverFolder)
		{
			var compositionServerExe = Path.Combine(serverFolder, "CodeEditor.Composition.Server.exe");
			System.Console.Error.WriteLine("Starting {0}", compositionServerExe);

			var timer = Stopwatch.StartNew();
			var serverProcess = StartServerProcess(compositionServerExe);
			serverProcess.StandardOutput.ReadLine(); // wait for the server to be up
			timer.Stop();

			System.Console.Error.WriteLine("{0} started in {1}ms", compositionServerExe, timer.ElapsedMilliseconds);

			return new CompositionServerController(serverProcess);
		}

		private IProcess StartServerProcess(string compositionServerExe)
		{
			return Shell.StartManagedProcess(compositionServerExe);
		}
	}

	public interface ICompositionServerController : IDisposable
	{
		int Id { get; }
		bool Stop(int timeout);
	}

	public class CompositionServerController : ICompositionServerController
	{
		public static ICompositionServerController StartCompositionServerAt(string serverFolder)
		{
			return new CompositionServerControllerFactory { Shell = new StandardShell() }.StartCompositionServerAtFolder(serverFolder);
		}

		private readonly IProcess _serverProcess;

		internal CompositionServerController(IProcess serverProcess)
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