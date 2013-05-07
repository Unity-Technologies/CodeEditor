using System;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.IO.Unity.Editor;
using CodeEditor.Logging;
using CodeEditor.Logging.Unity;
using CodeEditor.Reactive;
using CodeEditor.ReactiveServiceStack;
using CodeEditor.ServiceClient;

namespace CodeEditor.Text.UI.Unity.Editor
{
	public static class UnityEditorCompositionContainer
	{
		public static T GetExportedValue<T>()
		{
			return CompositionContainer.GetExportedValue<T>();
		}

		static CompositionContainer CompositionContainer
		{
			get { return Container.Value; }
		}

		static readonly Lazy<CompositionContainer> Container = new Lazy<CompositionContainer>(CreateCompositionContainer);

		static CompositionContainer CreateCompositionContainer()
		{
			var container = new CompositionContainer(AppDomain.CurrentDomain.GetAssemblies().ToArray());
			container.AddExportedValue<IFileSystem>(new UnityEditorFileSystem());
			container.AddExportedValue(new DataPathProvider(UnityEngine.Application.dataPath));
			container.AddExportedValue<IMonoExecutableProvider>(new UnityMonoExecutableProvider());
			if (UnityEngine.Debug.isDebugBuild)
				container.AddExportedValue<ILogger>(new UnityLogger());
			return container;
		}
	}

	class DataPathProvider
	{
		public ResourcePath DataPath { get; private set; }

		public DataPathProvider(ResourcePath dataPath)
		{
			DataPath = dataPath;
		}
	}

	[Export]
	class ProjectPathProvider
	{
		public ResourcePath ProjectPath { get; private set; }

		[ImportingConstructor]
		public ProjectPathProvider(DataPathProvider dataPathProvider)
		{
			ProjectPath = dataPathProvider.DataPath.Parent;
		}
	}

	[Export(typeof(ICodeEditorServiceClientProvider))]
	class CodeEditorServiceClientProvider : ICodeEditorServiceClientProvider
	{
		readonly Lazy<ProcessSettings> _serviceHostProcessSettings;

		public CodeEditorServiceClientProvider()
		{
			_serviceHostProcessSettings = new Lazy<ProcessSettings>(() => new ProcessSettings(ServiceHostExecutable));
		}

		public IObservableX<IObservableServiceClient> Client
		{
			get { return ServiceClientProvider.ClientFor(ServiceHostProcessSettings); }
		}

		[Import]
		public ProjectPathProvider ProjectPathProvider { get; set; }

		[Import]
		public IObservableServiceClientProvider ServiceClientProvider { get; set; }

		ResourcePath ServiceHostExecutable
		{
			get { return ProjectPath / "Library/CodeEditor/ServiceHost/CodeEditor.ServiceHost.exe"; }
		}

		ResourcePath ProjectPath
		{
			get { return ProjectPathProvider.ProjectPath; }
		}

		ProcessSettings ServiceHostProcessSettings
		{
			get { return _serviceHostProcessSettings.Value; }
		}
	}
}
