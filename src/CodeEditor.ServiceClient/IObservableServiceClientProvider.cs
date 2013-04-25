using System;
using System.Diagnostics;
using ServiceStack.Text;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;
using CodeEditor.ReactiveServiceStack;

namespace CodeEditor.ServiceClient
{
	public interface ICodeEditorServiceClientProvider
	{
		IObservableX<IObservableServiceClient> Client { get; }
	}

	public interface IObservableServiceClientProvider
	{
		IObservableX<IObservableServiceClient> ClientFor(ProcessSettings serviceHostProcessSettings);
	}

	[Export(typeof(IObservableServiceClientProvider))]
	public class ObservableServiceClientProvider : IObservableServiceClientProvider
	{
		[Import]
		public IFileSystem FileSystem { get; set; }

		[Import]
		public IShell Shell { get; set; }

		[Import]
		public ILogger Logger { get; set; }

		public IObservableX<IObservableServiceClient> ClientFor(ProcessSettings serviceHostProcessSettings)
		{
			return CreateClientFor(serviceHostProcessSettings).RetryEvery(TimeSpan.FromMilliseconds(500), 3);
		}

		IObservableX<IObservableServiceClient> CreateClientFor(ProcessSettings serviceHostProcessSettings)
		{
			return ObservableX.CreateWithDisposable<IObservableServiceClient>(observer =>
			{
				try
				{
					var serviceHostUri = ServiceHostUriFor(serviceHostProcessSettings);
					observer.CompleteWith(new ObservableServiceClient(serviceHostUri));
				}
				catch (Exception e)
				{
					Logger.LogError(e);
					observer.OnError(e);
				}
				return Disposable.Empty;
			});
		}

		string ServiceHostUriFor(ProcessSettings serviceHostProcessSettings)
		{
			lock (this)
			{
				var uriFile = UriFileFor(serviceHostProcessSettings.Executable);
				EnsureServiceHostProcessIsUp(uriFile, serviceHostProcessSettings);
				return FirstLineOf(uriFile);
			}
		}

		string FirstLineOf(IFile uriFile)
		{
			var content = uriFile.ReadAllText();
			if (string.IsNullOrEmpty(content))
				throw new InvalidOperationException("Server address couldn't be read.");
			return content.Trim();
		}

		void EnsureServiceHostProcessIsUp(IFile uriFile, ProcessSettings serviceHostProcessSettings)
		{
			if (!uriFile.TryToDelete())
			{
				Logger.Log("server is already running");
				return;
			}
			StartServiceHost(serviceHostProcessSettings);
			WaitFor(uriFile, TimeSpan.FromMilliseconds(200));
		}

		void WaitFor(IFile uriFile, TimeSpan timeout)
		{
			var timer = Stopwatch.StartNew();
			while (!uriFile.Exists() && timer.Elapsed < timeout)
				System.Threading.Thread.Sleep(0);
		}

		IFile UriFileFor(ResourcePath serviceHostExecutablePath)
		{
			return FileFor(UriFilePathFor(serviceHostExecutablePath));
		}

		IFile FileFor(ResourcePath path)
		{
			return FileSystem.FileFor(path);
		}

		void StartServiceHost(ProcessSettings serviceHostProcessSettings)
		{
			Logger.Log("Starting {0}".Fmt(serviceHostProcessSettings.Executable));
			using (Shell.StartManagedProcess(serviceHostProcessSettings))
			{
				// this doesn't kill the actual process but
				// just releases any resources attached to
				// the object
			}
		}

		ResourcePath UriFilePathFor(ResourcePath serviceHostExecutablePath)
		{
			return serviceHostExecutablePath.ChangeExtension("uri");
		}
	}
}