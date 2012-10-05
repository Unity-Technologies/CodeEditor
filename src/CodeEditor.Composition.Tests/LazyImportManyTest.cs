using System.Linq;
using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.Composition.Tests
{
	[TestFixture]
	public class LazyImportManyTest
	{
		[Test]
		public void OnlyServicesWithMatchingMetadataAreProvided()
		{
			var container = new CompositionContainer(GetType().Assembly);

			var service = container.GetExportedValue<ServiceWithImports>();
			Assert.IsNotNull(service.Imports);

			var expected = new[]
			{
				new {Type = typeof(Service1), Name = "Foo"},
				new {Type = typeof(Service2), Name = "Bar"}
			};
			var actual = service.Imports
				.Select(import => new {Type = import.Value.GetType(), import.Metadata.Name});

			CollectionAssert.AreEquivalent(expected, actual.ToArray());
		}

		[Export]
// ReSharper disable ClassNeverInstantiated.Local
		class ServiceWithImports
		{
			[ImportMany]
#pragma warning disable 649
			public Lazy<IService, IServiceMetadata>[] Imports;
#pragma warning restore 649
		}

		interface IService
		{
		}

		[ExportServiceWithMetadata("Foo")]
		class Service1 : IService
		{
		}

		[ExportServiceWithMetadata("Bar")]
		class Service2 : IService
		{
		}

		[Export(typeof(IService))]
// ReSharper disable UnusedMember.Local
		class ServiceWithoutMetadata : IService
// ReSharper restore UnusedMember.Local
		{
		}
		// ReSharper restore ClassNeverInstantiated.Local

		interface IServiceMetadata
		{
			string Name { get; }
		}

		class ExportServiceWithMetadata : ExportAttribute, IServiceMetadata
		{
			public string Name { get; private set; }

			public ExportServiceWithMetadata(string name) : base(typeof(IService))
			{
				Name = name;
			}
		}
	}
}
