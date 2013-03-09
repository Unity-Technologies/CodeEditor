using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Testing;
using CodeEditor.Text.Data;
using CodeEditor.Text.Data.Implementation;
using CodeEditor.Text.Logic;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Text.UI.Unity.Engine.Tests
{
	[TestFixture]
	public class TextViewDocumentFactoryTest : MockBasedTest
	{
		[Test]
		public void ContentTypeWithoutClassifierIsSupported()
		{
			var contentType = MockFor<IContentType>();
			contentType
				.Setup(c => c.GetService(typeof(IClassifierProvider)))
				.Returns(null);

			var textDocument = MockFor<ITextDocument>();
			textDocument
				.SetupGet(d => d.Buffer)
				.Returns(new TextBuffer("", contentType.Object));

			var file = MockFor<IFile>();
			var textDocumentFactory = MockFor<ITextDocumentFactory>();
			textDocumentFactory
			.Setup(f => f.ForFile(file.Object))
			.Returns(textDocument.Object);

			var factory = new TextViewDocumentFactory
			{
				TextDocumentFactory = textDocumentFactory.Object,
				ClassificationStyler = MockFor<IClassificationStyler>().Object,
				CaretFactory = MockFor<ICaretFactory>(MockBehavior.Loose).Object
			};

			var textViewDocument = factory.DocumentForFile(file.Object);
			Assert.IsNotNull(textViewDocument);

			VerifyAllMocks();
		}
	}
}
