using System.Reflection;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.ContentTypes.Tests
{
	[TestFixture]
	public class ContentTypeRegistryTest
	{
		IContentTypeRegistry _registry;

		[SetUp]
		public void SetUp()
		{
			var container = new CompositionContainer(
				AssemblyOf<IContentTypeSpecificService>(),
				AssemblyOf<IContentTypeRegistry>());
			_registry = container.GetExportedValue<IContentTypeRegistry>();
		}

		[Test]
		public void ForName()
		{
			var contentType = _registry.ForName(FooContentType.Name);
			Assert.AreEqual(FooContentType.Name, contentType.Name);
			Assert.IsInstanceOf<FooContentType>(contentType.Definition);
		}

		[Test]
		public void ContentTypeSpecificService()
		{
			var contentType = _registry.ForName(FooContentType.Name);
			Assert.IsInstanceOf<FooSpecificService>(contentType.GetService<IContentTypeSpecificService>());
		}

		[Test]
		public void TextContentTypeIsAvailable()
		{
			var contentType = _registry.ForName("text");
			Assert.AreEqual("text", contentType.Name);
			Assert.AreEqual("text", _registry.ForFileExtension(".txt").Name);
			Assert.AreEqual(new[] {".txt"}, _registry.FileExtensionsFor(contentType));
		}

		[Test]
		public void ForNameReturnsNullForUnknownContentType()
		{
			Assert.IsNull(_registry.ForName("bagoonfleskish"));
		}

		static Assembly AssemblyOf<T>()
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