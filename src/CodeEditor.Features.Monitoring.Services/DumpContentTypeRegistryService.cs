using System.Linq;
using CodeEditor.ContentTypes;
using ServiceStack.ServiceInterface;

namespace CodeEditor.Features.Monitoring.Services
{
	public class DumpContentTypeRegistryRequest
	{
	}

	public class DumpContentTypeRegistryService : Service
	{
		public IContentTypeRegistry ContentTypeRegistry { get; set; }

		public object Get(DumpContentTypeRegistryRequest request)
		{
			return ContentTypeRegistry
				.ContentTypes
				.Select(_ => new
				{
					ContentType = _.Name,
					FileExtensions = ContentTypeRegistry.FileExtensionsFor(_)
				});
		}
	}
}