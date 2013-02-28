using System;
using System.IO;
using CodeEditor.Composition;
using CodeEditor.Composition.Client;
using CodeEditor.IO;
using CodeEditor.Server.Interface;

namespace CodeEditor.Languages.Common
{
	public interface IUnityProjectProvider
	{
		IUnityProject Project { get; }
	}

	public interface IUnityProjectPathProvider
	{
		string Location { get; }
	}

	[Export(typeof(IUnityProjectProvider))]
	public class UnityProjectProvider : IUnityProjectProvider
	{
		private readonly Lazy<IUnityProject> _project;

		[Import]
		public IFileSystem FileSystem;

		[Import]
		public IUnityProjectPathProvider ProjectPathProvider;

		[Import]
		public ICompositionServerControllerFactory ControllerFactory;

		[Import]
		public ICompositionClientProvider ClientProvider;

		public UnityProjectProvider()
		{
			_project = new Lazy<IUnityProject>(GetProject);
		}

		public IUnityProject Project
		{
			get { return _project.Value; }
		}

		private IUnityProject GetProject()
		{
			EnsureCompositionServerIsRunning();

			var client = ClientProvider.CompositionClientFor(PidFile.ReadAllText());
			return client.GetService<IUnityProjectServer>().ProjectForFolder(ProjectFolder);
		}

		private IFile PidFile
		{
			get { return FileSystem.FileFor(PidFilePath); }
		}

		private void EnsureCompositionServerIsRunning()
		{
			if (IsRunning())
				return;
			StartCompositionContainer();
		}

		private void StartCompositionContainer()
		{
			ControllerFactory.StartCompositionServerAtFolder(Path.GetDirectoryName(CompositionServerExe));
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
				System.Console.WriteLine("pid file is active");
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