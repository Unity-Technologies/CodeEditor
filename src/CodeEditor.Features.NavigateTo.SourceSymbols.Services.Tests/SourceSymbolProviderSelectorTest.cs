using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.Tests
{
	[TestFixture]
	public class SourceSymbolProviderSelectorTest : MockBasedTest
	{
		[Test]
		public void SelectsContentTypeSpecificProvider()
		{
			const string fileExtension = ".ftw";

			var ftwFile = MockFor<IFile>();
			ftwFile
				.SetupGet(_ => _.Extension)
				.Returns(fileExtension);

			var symbols = new ISourceSymbol[0];
			var ftwParser = MockFor<ISourceSymbolProvider>();
			ftwParser
				.Setup(_ => _.SourceSymbolsFor(ftwFile.Object))
				.Returns(symbols);

			var contentTypeRegistry = MockFor<IContentTypeRegistry>();
			var contentType = MockFor<IContentType>();
			contentTypeRegistry
				.Setup(_ => _.ForFileExtension(fileExtension))
				.Returns(contentType.Object);

			contentType
				.Setup(_ => _.GetService(typeof(ISourceSymbolProvider)))
				.Returns(ftwParser.Object);

			var subject = new SourceSymbolProviderSelector {ContentTypeRegistry = contentTypeRegistry.Object};
			var parsedSymbols = subject.SourceSymbolsFor(ftwFile.Object);
			Assert.AreSame(symbols, parsedSymbols);

			VerifyAllMocks();
		}
	}
}