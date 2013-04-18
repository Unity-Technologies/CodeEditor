using System;
using ServiceStack.Text;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;
using CodeEditor.ReactiveServiceStack;

namespace CodeEditor.ServiceClient
{
	public interface IObservableServiceClientProvider
	{
		IObservableX<IObservableServiceClient> Client { get; }
	}

	[Export(typeof(IObservableServiceClientProvider))]
	public class ObservableServiceClientProvider : IObservableServiceClientProvider
	{
		readonly Lazy<IFile> _serviceHostUriFile;

		public ObservableServiceClientProvider()
		{
			_serviceHostUriFile = new Lazy<IFile>(() => FileSystem.GetFile(ServiceHostUriFilePath));
		}

		[Import]
		public IServiceHostExecutableProvider ServiceHostExecutableProvider { get; set; }

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
							.Never<IObservableServiceClient>()
							.Timeout(
								TimeSpan.FromMilliseconds(500),
								ObservableX.Throw<IObservableServiceClient>(e)))
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
			get { return _serviceHostUriFile.Value; }
		}

		void StartCompositionContainer()
		{
			var executable = ServiceHostExecutable;
			Logger.Log("Starting {0}".Fmt(executable));
			using (Shell.StartManagedProcess(executable.Location))
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

		ResourcePath ServiceHostUriFilePath
		{
			get { return ServiceHostExecutable.ChangeExtension("uri"); }
		}

		ResourcePath ServiceHostExecutable
		{
			get { return ServiceHostExecutableProvider.ServiceHostExecutable; }
		}
	}

	public interface IServiceHostExecutableProvider
	{
		string ServiceHostExecutable { get; }
	}

	public class ServiceHostExecutableProvider : IServiceHostExecutableProvider
	{
		readonly string _serviceHostExecutable;

		public ServiceHostExecutableProvider(string serviceHostExecutable)
		{
			_serviceHostExecutable = serviceHostExecutable;
		}

		public string ServiceHostExecutable
		{
			get { return _serviceHostExecutable; }
		}
	}
}