using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.Tests
{
	[TestFixture]
	public class SymbolParserSelectorTest : MockBasedTest
	{
		[Test]
		public void ParsesUsingContentTypeSpecificParser()
		{
			const string fileExtension = ".ftw";

			var ftwFile = MockFor<IFile>();
			ftwFile
				.SetupGet(_ => _.Extension)
				.Returns(fileExtension);

			var symbols = new ISymbol[0];
			var ftwParser = MockFor<ISymbolParser>();
			ftwParser
				.Setup(_ => _.Parse(ftwFile.Object))
				.Returns(symbols);

			var contentTypeRegistry = MockFor<IContentTypeRegistry>();
			var contentType = MockFor<IContentType>();
			contentTypeRegistry
				.Setup(_ => _.ForFileExtension(fileExtension))
				.Returns(contentType.Object);

			contentType
				.Setup(_ => _.GetService(typeof(ISymbolParser)))
				.Returns(ftwParser.Object);

			var subject = new SymbolParserSelector {ContentTypeRegistry = contentTypeRegistry.Object};
			var parsedSymbols = subject.Parse(ftwFile.Object);
			Assert.AreSame(symbols, parsedSymbols);

			VerifyAllMocks();
		}
	}
}