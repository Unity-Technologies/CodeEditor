using System;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[InitializeOnLoad]
	public static class Main
	{
		static Main()
		{
			CodeEditorWindow.TextViewFactory = ViewForFile;
			NavigatorWindow.ProviderAggregatorFactory = GetExportedValue<INavigateToItemProviderAggregator>;
		}

		private static ITextView ViewForFile(string fileName)
		{
			return GetExportedValue<ITextViewFactory>().ViewForFile(fileName);
		}

		private static T GetExportedValue<T>()
		{
			return CodeTimer.LoggingTime(
				string.Format("GetExportedValue<{0}>()", typeof(T).Name),
				() => CompositionContainer.GetExportedValue<T>());
		}

		private static CompositionContainer CompositionContainer
		{
			get { return Container.Value; }
		}

		private static readonly Lazy<CompositionContainer> Container = new Lazy<CompositionContainer>(CreateCompositionContainer);

		private static CompositionContainer CreateCompositionContainer()
		{
			var container = new CompositionContainer(AppDomain.CurrentDomain.GetAssemblies().ToArray());
			container.AddExportedValue<IFileSystem>(new UnityEditorFileSystem());
			container.AddExportedValue<ILogger>(new UnityLogger());
			return container;
		}

		private class UnityLogger : ILogger
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
