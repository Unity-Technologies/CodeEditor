using System;
using System.Threading;
using CodeEditor.Composition.Hosting;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.Tests
{
	[TestFixture]
	public class SourceSymbolIndexProviderTest : MockBasedTest
	{
		[Test]
		public void IndexAllFilesAutomaticallyUponStartup()
		{
			var sourceFile = MockFor<IFile>().Object;
			var sourceFileNotification = MockFor<IFileNotification>();
			sourceFileNotification
				.SetupGet(_ => _.File)
				.Returns(sourceFile);

			var sourceFilesProvider = MockFor<ISourceFilesProvider>();
			sourceFilesProvider
				.SetupGet(_ => _.SourceFiles)
				.Returns(ObservableX.Return(sourceFileNotification.Object));

			var providerSelector = MockFor<ISourceSymbolProviderSelector>();
			var parseWaitEvent = new AutoResetEvent(false);
			var symbol = MockFor<ISourceSymbol>();
			providerSelector
				.Setup(_ => _.SourceSymbolsFor(sourceFile))
				.Callback(() => parseWaitEvent.Set())
				.Returns(new[] {symbol.Object});

			var container = new CompositionContainer(typeof(ISourceSymbolIndexProvider).Assembly);
			container.AddExportedValue(sourceFilesProvider.Object);
			container.AddExportedValue(providerSelector.Object);
			container.AddExportedValue<ILogger>(new StandardLogger());

			var subject = container.GetExportedValue<ISourceSymbolIndexProvider>();
			Assert.IsNotNull(subject.Index);

			// TODO: replace by injecting immediate scheduler
			parseWaitEvent.WaitOne(TimeSpan.FromSeconds(1));

			VerifyAllMocks();
		}
	}
}
