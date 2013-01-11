using System;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
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
			CodeEditorWindow.TextViewFactory = new LazyTextViewFactory();
			NavigatorWindow.ProviderFactory = () => CompositionContainer.GetExportedValue<INavigateToItemProvider>();
		}

		private class LazyTextViewFactory : ITextViewFactory
		{
			public ITextView ViewForFile(string fileName)
			{
				return ActualTextViewFactory.ViewForFile(fileName);
			}

			public ITextView CreateView()
			{
				throw new NotImplementedException();
			}

			ITextViewFactory ActualTextViewFactory
			{
				get { return CodeTimer.LoggingTime("GetExportedValue<ITextViewFactory>()", () => CompositionContainer.GetExportedValue<ITextViewFactory>()); }
			}
		}

		private static CompositionContainer CompositionContainer
		{
			get { return _container.Value; }
		}

		private static CompositionContainer CreateCompositionContainer()
		{
			var container = new CompositionContainer(AppDomain.CurrentDomain.GetAssemblies().ToArray());
			container.AddExportedValue<IFileSystem>(new UnityEditorFileSystem());
			return container;
		}

		private static readonly Lazy<CompositionContainer> _container = new Lazy<CompositionContainer>(CreateCompositionContainer);
	}
}
