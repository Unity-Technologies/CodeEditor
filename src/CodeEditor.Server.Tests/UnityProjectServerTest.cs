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
			var assetsFolder = MockFor<IFolder>();
			var sourceFile = MockFor<IFile>();
			assetsFolder
				.Setup(_ => _.GetFiles("*.cs", SearchOption.AllDirectories))
				.Returns(new[] {sourceFile.Object});

			var assetsFolderProvider = MockFor<IUnityAssetsFolderProvider>();
			assetsFolderProvider
				.SetupGet(_ => _.AssetsFolder)
				.Returns(assetsFolder.Object);

			var parser = MockFor<ISymbolParser>();
			var parseWaitEvent = new AutoResetEvent(false);
			var symbol = MockFor<ISymbol>();
			parser
				.Setup(_ => _.Parse(sourceFile.Object))
				.Callback(() => parseWaitEvent.Set())
				.Returns(new[] {symbol.Object});

			var container = new CompositionContainer(typeof(IUnityProjectProvider).Assembly);
			container.AddExportedValue(assetsFolderProvider.Object);
			container.AddExportedValue(parser.Object);

			var subject = container.GetExportedValue<IUnityProjectProvider>();
			var project = subject.Project;

			// TODO: replace by injecting immediate scheduler
			parseWaitEvent.WaitOne(TimeSpan.FromSeconds(1));

			VerifyAllMocks();
		}
	}
}
