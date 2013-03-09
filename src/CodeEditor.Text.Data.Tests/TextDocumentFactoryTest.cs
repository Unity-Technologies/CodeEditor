using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Testing;
using CodeEditor.Text.Data.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextDocumentFactoryTest : MockBasedTest
	{
		[Test]
		public void ContentTypeIsDeducedFromFileExtension()
		{
			const string fileExtension = ".ext";

			var file = MockFor<IFile>(MockBehavior.Loose);
			file.SetupGet(_ => _.Extension).Returns(fileExtension);
			file.Setup(_ => _.ReadAllText()).Returns("text");

			var contentType = MockFor<IContentType>();

			var contentTypeRegistry = MockFor<IContentTypeRegistry>();
			contentTypeRegistry
				.Setup(_ => _.ForFileExtension(fileExtension))
				.Returns(contentType.Object);

			var factory = new TextDocumentFactory { ContentTypeRegistry = contentTypeRegistry.Object };
			var document = factory.ForFile(file.Object);
			Assert.AreSame(contentType.Object, document.Buffer.ContentType);

			VerifyAllMocks();
		}
	}
}
