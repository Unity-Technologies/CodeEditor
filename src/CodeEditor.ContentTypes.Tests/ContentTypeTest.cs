using CodeEditor.Composition.Primitives;
using CodeEditor.ContentTypes.Internal;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.ContentTypes.Tests
{
	[TestFixture]
	public class ContentTypeTest : MockBasedTest
	{
		[Test]
		public void GetServiceReturnsNullWhenNoServiceCanBeFound()
		{
			var exportProvider = MockFor<IExportProvider>();
			exportProvider
				.Setup(p => p.GetExports(typeof(IService)))
				.Returns(new Export[0]);

			var contentType = new ContentType(exportProvider.Object, "", null);
			Assert.IsNull(contentType.GetService<IService>());
		}

		interface IService
		{
		}
	}
}
