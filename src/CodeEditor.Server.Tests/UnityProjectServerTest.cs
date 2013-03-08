using System;
using System.IO;
using System.Threading;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Server.Tests
{
	[TestFixture]
	public class UnityProjectServerTest : MockBasedTest
	{
		[Test]
		public void ParsesAllFilesAutomaticallyUponStartup()
		{
			var fileSystem = MockFor<IFileSystem>();
			var projectFolder = MockFor<IFolder>();
			fileSystem
				.Setup(_ => _.FolderFor(Path.GetFullPath("UnityProject")))
				.Returns(projectFolder.Object);

			var sourceFile = MockFor<IFile>();
			projectFolder
				.Setup(_ => _.GetFiles("*.cs", SearchOption.AllDirectories))
				.Returns(new[] {sourceFile.Object});

			var parser = MockFor<ISymbolParser>();

			var parseWaitEvent = new AutoResetEvent(false);
			var symbol = MockFor<ISymbol>();
			parser
				.Setup(_ => _.Parse(sourceFile.Object))
				.Callback(() => parseWaitEvent.Set())
				.Returns(new[] {symbol.Object});

			var container = new CompositionContainer(typeof(UnityProjectFactory).Assembly);
			container.AddExportedValue(fileSystem.Object);
			container.AddExportedValue(parser.Object);

			var subject = container.GetExportedValue<IUnityProjectFactory>();
			var project = subject.ProjectForFolder("UnityProject");

			// TODO: replace by injecting immediate scheduler
			parseWaitEvent.WaitOne(TimeSpan.FromSeconds(1));

			VerifyAllMocks();
		}
	}
}
