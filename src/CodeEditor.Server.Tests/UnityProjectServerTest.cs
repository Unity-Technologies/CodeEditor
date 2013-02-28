using System.IO;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.Reactive;
using CodeEditor.Server.Interface;
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
				.Setup(_ => _.FolderFor("UnityProject"))
				.Returns(projectFolder.Object);

			var sourceFile = MockFor<IFile>();
			projectFolder
				.Setup(_ => _.GetFiles("*.cs", SearchOption.AllDirectories))
				.Returns(new[] {sourceFile.Object});

			var parser = MockFor<ISymbolParser>();
			var symbol = MockFor<ISymbol>();
			parser
				.Setup(_ => _.Parse(sourceFile.Object))
				.Returns(new[] {symbol.Object});
			symbol
				.SetupGet(_ => _.DisplayText)
				.Returns("");

			var container = new CompositionContainer(typeof(UnityProjectServer).Assembly);
			container.AddExportedValue(fileSystem.Object);
			container.AddExportedValue(parser.Object);

			var subject = container.GetExportedValue<IUnityProjectServer>();
			var project = subject.ProjectForFolder("UnityProject");
			Assert.AreSame(symbol.Object, project.SearchSymbol("").FirstOrDefault());

			VerifyAllMocks();
		}
	}
}
