using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.Composition.Tests
{
	[TestFixture]
	public class LazyImportTest
	{
		[Test]
		public void MetadataIsProvided()
		{
			var container = new CompositionContainer(GetType().Assembly);

			var service = container.GetExportedValue<ServiceWithLazyImport>();

			Assert.IsNotNull(service.Import);
			Assert.AreEqual(42, service.Import.Metadata.Value);

			Assert.IsNotNull(service.Import.Value);
			Assert.AreSame(service.Import.Value, service.Import.Value);
		}

		[Export]
		public class ServiceWithLazyImport
		{
			[Import]
			public Lazy<ServiceWithMetadata, IServiceMetadata> Import;
		}

		[ExportWithMetadata(42)]
		public class ServiceWithMetadata
		{
		}

		public interface IServiceMetadata
		{
			int Value { get; }
		}

		class ExportWithMetadataAttribute : ExportAttribute, IServiceMetadata
		{
			public int Value { get; private set; }

			public ExportWithMetadataAttribute(int value)
			{
				Value = value;
			}
		}
	}
}
