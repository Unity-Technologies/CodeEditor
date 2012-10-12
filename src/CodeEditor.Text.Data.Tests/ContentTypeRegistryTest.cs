using System.Reflection;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class ContentTypeRegistryTest
	{
		private IContentTypeRegistry _registry;

		[SetUp]
		public void SetUp()
		{
			var container = new CompositionContainer(AssemblyOf<IContentTypeSpecificService>(), AssemblyOf<IContentTypeRegistry>());
			_registry = container.GetExportedValue<IContentTypeRegistry>();
		}

		[Test]
		public void ContentTypeSpecificService()
		{
			var contentType = _registry.ForName(FooContentType.Name);
			Assert.AreEqual(FooContentType.Name, contentType.Name);
			Assert.IsInstanceOf<FooContentType>(contentType.Definition);
			Assert.IsInstanceOf<FooSpecificService>(contentType.GetService<IContentTypeSpecificService>());
		}

		[Test]
		public void TextContentTypeIsAvailable()
		{
			Assert.AreEqual("text", _registry.ForName("text").Name);
			Assert.AreEqual("text", _registry.ForFileExtension(".txt").Name);
		}

		[Test]
		public void ForNameReturnsNullForUnknownContentType()
		{
			Assert.IsNull(_registry.ForName("bagoonfleskish"));
		}

		private static Assembly AssemblyOf<T>()
		{
			return typeof(T).Assembly;
		}
	}

	/// <summary>
	/// A service with a different implementation
	/// for each content type.
	/// </summary>
	public interface IContentTypeSpecificService
	{
	}

	[ContentTypeDefinition(Name)]
	public class FooContentType : IContentTypeDefinition
	{
		public const string Name = "Foo";
	}

	[Export(typeof(IContentTypeSpecificService))]
	[ContentType(FooContentType.Name)]
	public class FooSpecificService : IContentTypeSpecificService
	{
	}

	[ContentTypeDefinition(Name)]
	public class BarContentType : IContentTypeDefinition
	{
		public const string Name = "Bar";
	}

	[Export(typeof(IContentTypeSpecificService))]
	[ContentType(BarContentType.Name)]
	public class BarSpecificService : IContentTypeSpecificService
	{
	}
}
