using System;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.IO.Unity.Editor;
using CodeEditor.Logging;
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
			container.AddExportedValue<IServiceHostExecutableProvider>(new ServiceHostExecutableProvider(ServerExecutable));
			container.AddExportedValue<IMonoExecutableProvider>(new UnityMonoExecutableProvider());
			if (UnityEngine.Debug.isDebugBuild)
				container.AddExportedValue<ILogger>(new UnityLogger());
			return container;
		}

		static string ServerExecutable
		{
			get { return Path.Combine(ProjectPath, "Library/CodeEditor/Services/CodeEditor.ServiceHost.exe"); }
		}

		static string ProjectPath
		{
			get { return Path.GetDirectoryName(UnityEngine.Application.dataPath); }
		}

		class UnityLogger : ILogger
		{
			public void Log(object value)
			{
				UnityEngine.Debug.Log(value);
			}

			public void LogError(Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}
}