using System.IO;
using System.Linq;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Reactive;
using CodeEditor.Testing;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.Tests
{
	[TestFixture]
	public class SourceFilesProviderTest : MockBasedTest
	{
		[Test]
		public void EnumeratesSourceFilesOfContentTypesWithSourceSymbolProviderService()
		{
			// cs and js have associated ISourceSymbolProvider
			var cs = ContentTypeWithSourceSymbolProvider();
			var js = ContentTypeWithSourceSymbolProvider();

			// txt doesn't have an associated ISourceSymbolProvider
			var txt = MockFor<IContentType>(MockBehavior.Loose).Object;

			var contentTypeRegistry = MockFor<IContentTypeRegistry>();
			contentTypeRegistry
				.SetupGet(_ => _.ContentTypes)
				.Returns(new[] {txt, cs, js});
			contentTypeRegistry
				.Setup(_ => _.FileExtensionsFor(cs))
				.Returns(new[] {".cs"});
			contentTypeRegistry
				.Setup(_ => _.FileExtensionsFor(js))
				.Returns(new[] {".js", ".us"});

			var sourceFolderProvider = MockFor<IUnityAssetsFolderProvider>();
			var sourceFolder = MockFor<IFolder>();
			sourceFolderProvider
				.SetupGet(_ => _.AssetsFolder)
				.Returns(sourceFolder.Object);
			var csFile = MockFor<IFile>().Object;
			sourceFolder
				.Setup(_ => _.SearchFiles("*.cs", SearchOption.AllDirectories))
				.Returns(new[] {csFile});
			var jsFile = MockFor<IFile>().Object;
			sourceFolder
				.Setup(_ => _.SearchFiles("*.js", SearchOption.AllDirectories))
				.Returns(new[] {jsFile});
			var usFile = MockFor<IFile>().Object;
			sourceFolder
				.Setup(_ => _.SearchFiles("*.us", SearchOption.AllDirectories))
				.Returns(new[] {usFile});

			var subject = new SourceFilesProvider
			{
				ContentTypeRegistry = contentTypeRegistry.Object,
				SourceFolderProvider = sourceFolderProvider.Object
			};
			var fileNotifications = subject.SourceFiles.Take(3).ToEnumerable();
			CollectionAssert.AreEquivalent(
				new[] {
					new { File = csFile, NotificationKind = FileNotificationKind.New },
					new { File = jsFile, NotificationKind = FileNotificationKind.New },
					new { File = usFile, NotificationKind = FileNotificationKind.New }},
				fileNotifications.Select(_ => new { _.File, _.NotificationKind }));
		}

		IContentType ContentTypeWithSourceSymbolProvider()
		{
			var contentType = MockFor<IContentType>();
			contentType
				.Setup(_ => _.GetService(typeof(ISourceSymbolProvider)))
				.Returns(MockFor<ISourceSymbolProvider>().Object);
			return contentType.Object;
		}
	}
}