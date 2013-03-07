using System;
using System.IO;
using CodeEditor.Composition;
using CodeEditor.Composition.Client;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Server.Interface;
using ServiceStack.Text;
using IFile = CodeEditor.IO.IFile;

namespace CodeEditor.Languages.Common
{
	public interface IObservableServiceClientProvider
	{
		IObservableServiceClient Client { get; }
	}

	public interface IUnityProjectPathProvider
	{
		string Location { get; }
	}

	[Export(typeof(IObservableServiceClientProvider))]
	public class ObservableServiceClientProvider : IObservableServiceClientProvider
	{
		private readonly Lazy<IObservableServiceClient> _client;

		[Import]
		public IFileSystem FileSystem;

		[Import]
		public IUnityProjectPathProvider ProjectPathProvider;

		[Import]
		public ICompositionServerControllerFactory ControllerFactory;

		[Import]
		public ILogger Logger;

		public ObservableServiceClientProvider()
		{
			_client = new Lazy<IObservableServiceClient>(CreateClient);
		}

		public IObservableServiceClient Client
		{
			get { return _client.Value; }
		}

		private IObservableServiceClient CreateClient()
		{
			try
			{
				EnsureCompositionServerIsRunning();

				var baseUri = PidFile.ReadAllText();
				return new ObservableServiceClient(baseUri);
			}
			catch (Exception e)
			{
				Logger.LogError(e);
				throw;
			}
		}

		private IFile PidFile
		{
			get { return FileSystem.FileFor(PidFilePath); }
		}

		private void EnsureCompositionServerIsRunning()
		{
			if (IsRunning())
			{
				Logger.Log("server is already running");
				return;
			}
			StartCompositionContainer();
		}

		private void StartCompositionContainer()
		{
			var folder = Path.GetDirectoryName(CompositionServerExe);
			Logger.Log("Starting server at {0}".Fmt(folder));
			ControllerFactory.StartCompositionServerAtFolder(folder);
		}

		private bool IsRunning()
		{
			return !TryToDeleteFilePidFile();
		}

		private bool TryToDeleteFilePidFile()
		{
			try
			{
				PidFile.Delete();
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		private string PidFilePath
		{
			get { return Path.ChangeExtension(CompositionServerExe, "pid"); }
		}

		private string CompositionServerExe
		{
			get { return Path.Combine(ProjectFolder, "Library/CodeEditor/Server/CodeEditor.Composition.Server.exe"); }
		}

		protected string ProjectFolder
		{
			get { return ProjectPathProvider.Location; }
		}
	}
}