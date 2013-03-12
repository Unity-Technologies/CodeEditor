using System;
using System.Linq;
using CodeEditor.Composition.Hosting;
using CodeEditor.Composition.Primitives;
using NUnit.Framework;

namespace CodeEditor.Composition.Tests
{
	[TestFixture]
	public class CompositionContainerTest
	{
		private CompositionContainer _container;

		[SetUp]
		public void SetUp()
		{
			_container = new CompositionContainer(GetType().Assembly);
		}

		[Test]
		public void ContainerIsExportedAsExportProvider()
		{
			Assert.AreSame(_container, GetExportedValue<IExportProvider>());
		}

		[Test]
		public void GetExportedValueAlwaysReturnSameValue()
		{
			var service = GetExportedValue<AService>();
			Assert.IsNotNull(service);
			Assert.AreSame(service, GetExportedValue<AService>());
		}

		[Test]
		public void GetExportedValueSatisfiesPropertyImports()
		{
			Assert.AreSame(GetExportedValue<AService>(), GetExportedValue<AServiceWithDependencies>().AService);
		}

		[Test]
		public void GetExportedValueSatisfiesFieldImports()
		{
			Assert.AreSame(GetExportedValue<AService>(), GetExportedValue<AServiceWithFieldDependencies>().AService);
		}

		[Test]
		public void GetExportedValueSatisfiesInterfaceImportImplementedByInternalType()
		{
			Assert.AreSame(GetExportedValue<IService>(), GetExportedValue<IServiceWithDependencies>().AService);
		}

		[Test]
		public void GetExportedValueSatisfiesImportingConstructor()
		{
			Assert.AreSame(GetExportedValue<IService>(), GetExportedValue<ServiceWithImportingConstructor>().AService);
		}

		[Test]
		public void PartCanExportMultipleContracts()
		{
			Assert.AreSame(GetExportedValue<IContract1>(), GetExportedValue<IContract2>());
			Assert.IsTrue(GetExportedValue<IContract1>() is PartWithMultipleContracts);
		}

		[Test]
		public void CanAddExportedValueWithoutMetadata()
		{
			const string value = "42";
			_container.AddExportedValue(value);

			var export = _container.GetExports(typeof(string)).Single();
			Assert.AreEqual(value, export.Value);
			Assert.IsFalse(export.Metadata.Any(), "No metadata for added exported value");
		}

		[Test]
		public void CanAddExportedValueWithMetadata()
		{
			const string value = "42";
			const string metadata = "LTUAE";

			_container.AddExportedValue(value, metadata);

			var export = _container.GetExports(typeof(string)).Single();
			Assert.AreEqual(value, export.Value);
			Assert.AreEqual(metadata, export.Metadata.Single());
		}

		[Test]
		[TestCase(typeof(INonInheritedContract), typeof(BaseTypeWithNonInheritedContract))]
		[TestCase(typeof(BaseTypeWithNonInheritedContract), typeof(BaseTypeWithNonInheritedContract))]
		public void ExportedContractIsNotInherited(Type contractType, Type expectedImplementation)
		{
			var exports = _container.GetExports(contractType).ToList();
			Assert.AreEqual(1, exports.Count);
			Assert.AreSame(expectedImplementation, exports[0].Definition.Implementation);
		}

		[Test]
		[TestCase(typeof(IInheritedContract))]
		[TestCase(typeof(BaseTypeWithInheritedContract))]
		public void ExportedContractIsNotInherited(Type contractType)
		{
			var expectedImplementations = new[] {typeof(BaseTypeWithInheritedContract), typeof(DerivedTypeWithInheritedContract)};
			var actualImplementations = _container.GetExports(contractType).Select(_ => _.Definition.Implementation);
			CollectionAssert.AreEquivalent(expectedImplementations, actualImplementations);
		}

		private T GetExportedValue<T>()
		{
			return _container.GetExportedValue<T>();
		}

		// ReSharper disable ClassNeverInstantiated.Global
		[Export]
		public class AService
		{
		}

		[Export]
		public class AServiceWithDependencies
		{
			[Import]
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public AService AService { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}

		[Export]
		public class AServiceWithFieldDependencies
		{
			[Import]
			public AService AService;
		}

		public interface IServiceWithDependencies
		{
			IService AService { get; }
		}

		public interface IService
		{
		}

		// ReSharper disable UnusedMember.Global
		[Export(typeof(IService))]
		internal class ServiceImpl : IService
		{
		}

		[Export(typeof(IServiceWithDependencies))]
		internal class ServiceWithDependenciesImpl : IServiceWithDependencies
		{
			[Import]
			// ReSharper disable UnusedAutoPropertyAccessor.Global
			public IService AService { get; set; }
			// ReSharper restore UnusedAutoPropertyAccessor.Global
		}
		// ReSharper restore UnusedMember.Global

		[Export]
		public class ServiceWithImportingConstructor
		{
			[ImportingConstructor]
			ServiceWithImportingConstructor(IService service)
			{
				AService = service;
			}

			public IService AService { get; private set; }
		}
		// ReSharper restore ClassNeverInstantiated.Global

		public interface IContract1
		{
		}

		public interface IContract2
		{
		}

		[Export(typeof(IContract1))]
		[Export(typeof(IContract2))]
		public class PartWithMultipleContracts : IContract1, IContract2
		{
		}

		[Export(typeof(INonInheritedContract))]
		[Export]
		public class BaseTypeWithNonInheritedContract : INonInheritedContract
		{
		}

		public class DerivedTypeWithNonInheritedContract : BaseTypeWithNonInheritedContract
		{
		}

		public interface INonInheritedContract
		{
		}

		[InheritedExport(typeof(IInheritedContract))]
		[InheritedExport]
		public class BaseTypeWithInheritedContract : IInheritedContract
		{
		}

		public class DerivedTypeWithInheritedContract : BaseTypeWithInheritedContract
		{
		}

		public interface IInheritedContract
		{
		}
	}
}
