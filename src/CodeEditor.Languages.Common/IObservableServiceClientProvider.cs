using System;
using System.IO;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;
using CodeEditor.Server.Interface;
using ServiceStack.Text;
using IFile = CodeEditor.IO.IFile;

namespace CodeEditor.Languages.Common
{
	public interface IObservableServiceClientProvider
	{
		IObservableX<IObservableServiceClient> Client { get; }
	}

	[Export(typeof(IObservableServiceClientProvider))]
	public class ObservableServiceClientProvider : IObservableServiceClientProvider
	{
		readonly Lazy<IFile> _serverUriFile;

		public ObservableServiceClientProvider()
		{
			_serverUriFile = new Lazy<IFile>(() => FileSystem.FileFor(ServerUriFilePath));
		}

		[Import]
		public IServerExecutableProvider ServerExecutableProvider { get; set; }

		[Import]
		public IFileSystem FileSystem { get; set; }

		[Import]
		public IShell Shell { get; set; }

		[Import]
		public ILogger Logger { get; set; }

		public IObservableX<IObservableServiceClient> Client
		{
			get
			{
				return
					CreateClient()
					.Catch(
						(Exception e) =>
							ObservableX
							.Throw<IObservableServiceClient>(e)
							.Delay(TimeSpan.FromMilliseconds(500)))
					.Retry();
			}
		}

		IObservableX<IObservableServiceClient> CreateClient()
		{
			return ObservableX.CreateWithDisposable<IObservableServiceClient>(observer =>
			{
				try
				{
					EnsureCompositionServerIsRunning();
					var baseUri = FirstLineFromUriFile();
					observer.CompleteWith(new ObservableServiceClient(baseUri));
				}
				catch (Exception e)
				{
					Logger.LogError(e);
					observer.OnError(e);
				}
				return Disposable.Empty;
			});
		}

		string FirstLineFromUriFile()
		{
			var content = UriFile.ReadAllText();
			if (string.IsNullOrEmpty(content))
				throw new InvalidOperationException("Server address couldn't be read.");
			return content.Trim();
		}

		void EnsureCompositionServerIsRunning()
		{
			if (IsRunning())
			{
				Logger.Log("server is already running");
				return;
			}
			StartCompositionContainer();
		}

		IFile UriFile
		{
			get { return _serverUriFile.Value; }
		}

		void StartCompositionContainer()
		{
			var serverExe = ServerExecutable;
			Logger.Log("Starting {0}".Fmt(serverExe));
			using (Shell.StartManagedProcess(serverExe))
			{
				// this doesn't kill the actual process but
				// just releases any resources attached to
				// the object
			}
		}

		bool IsRunning()
		{
			return !TryToDeleteUriFile();
		}

		bool TryToDeleteUriFile()
		{
			try
			{
				UriFile.Delete();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		string ServerUriFilePath
		{
			get { return Path.ChangeExtension(ServerExecutable, "uri"); }
		}

		string ServerExecutable
		{
			get { return ServerExecutableProvider.ServerExecutable; }
		}
	}

	public interface IServerExecutableProvider
	{
		string ServerExecutable { get; }
	}

	public class ServerExecutableProvider : IServerExecutableProvider
	{
		readonly string _serverExecutable;

		public ServerExecutableProvider(string serverExecutable)
		{
			_serverExecutable = serverExecutable;
		}

		public string ServerExecutable
		{
			get { return _serverExecutable; }
		}
	}
}