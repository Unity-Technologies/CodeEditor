using System.Linq;
using CodeEditor.Composition.Primitives;
using NUnit.Framework;

namespace CodeEditor.Composition.Tests
{
	[TestFixture]
	public class ImportDefinitionProviderTest
	{
		[Test]
		public void LazyImportWithMetadata()
		{
			var provider = new ImportDefinitionProvider();
			var import = provider.ImportsFor(typeof(ClassWithLazyImportWithMetadata)).Single();
			Assert.AreEqual(typeof(IService), import.ContractType);
			Assert.AreEqual(ImportCardinality.One, import.Cardinality);
		}

		[Test]
		public void LazyImport()
		{
			var provider = new ImportDefinitionProvider();
			var import = provider.ImportsFor(typeof(ClassWithLazyImport)).Single();
			Assert.AreEqual(typeof(IService), import.ContractType);
			Assert.AreEqual(ImportCardinality.One, import.Cardinality);
		}

		[Test]
		public void LazyImportMany()
		{
			var provider = new ImportDefinitionProvider();
			var import = provider.ImportsFor(typeof(ClassWithLazyImportMany)).Single();
			Assert.AreEqual(typeof(IService), import.ContractType);
			Assert.AreEqual(ImportCardinality.Many, import.Cardinality);
		}

		public class ClassWithLazyImportWithMetadata
		{
			[Import]
			public Lazy<IService, IServiceMetadata> Service;
		}

		public class ClassWithLazyImport
		{
			[Import]
			public Lazy<IService> Service;
		}

		public class ClassWithLazyImportMany
		{
			[ImportMany]
			public Lazy<IService, IServiceMetadata>[] Services;
		}

		public interface IService
		{
		}

		public interface IServiceMetadata
		{
		}
	}
}
