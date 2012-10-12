using System;
using CodeEditor.Composition;

namespace CodeEditor.Text.Data
{
	public interface IContentType
	{
		string Name { get; }

		IContentTypeDefinition Definition { get; }

		/// <summary>
		/// Retrieves a content type specific service.
		///
		/// The export must be decorated with the <see cref="ContentTypeAttribute">ContentType</see>
		/// metadata attribute:
		///
		/// <example>
		/// [ExportDefinition(typeof(ICompletionProviderFactory))]
		/// [ContentType(CSharpContentType.Name)]
		/// class CSharpCompletionProviderFactory : ICompletionProviderFactory { ... }
		/// </example>
		/// </summary>
		object GetService(Type contractType);
	}

	public static class ContentTypeExtensions
	{
		public static T GetService<T>(this IContentType contentType)
		{
			return (T)contentType.GetService(typeof(T));
		}
	}

	/// <summary>
	/// Register a new content type definition.
	///
	/// [ContentTypeDefinition("Foo")]
	/// class FooContentType : IContentTypeDefinition {}
	/// </summary>
	public interface IContentTypeDefinition
	{
	}

	public interface IContentTypeDefinitionMetadata
	{
		string ContentTypeName { get; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ContentTypeDefinitionAttribute : ExportAttribute, IContentTypeDefinitionMetadata
	{
		public ContentTypeDefinitionAttribute(string contentTypeName) : base(typeof(IContentTypeDefinition))
		{
			ContentTypeName = contentTypeName;
		}

		public string ContentTypeName { get; private set; }
	}

	/// <summary>
	/// Extension point for new file extension to content type associations.
	///
	/// [FileExtensionToContentType("Foo", "foo")]
	/// class FooFileExtension : IFileExtensionToContentTypeDefinition {}
	/// </summary>
	public interface IFileExtensionToContentTypeDefinition
	{
	}

	public interface IFileExtensionToContentTypeMetadata
	{
		string FileExtension { get; }
		string ContentType { get; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class FileExtensionToContentTypeAttribute : ExportAttribute, IFileExtensionToContentTypeMetadata
	{
		public FileExtensionToContentTypeAttribute(string contentType, string fileExtension) : base(typeof(IFileExtensionToContentTypeDefinition))
		{
			ContentType = contentType;
			FileExtension = fileExtension;
		}

		public string FileExtension { get; private set; }
		public string ContentType { get; private set; }
	}

	public interface IContentTypeMetadata
	{
		string Name { get; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ContentTypeAttribute : Attribute, IContentTypeMetadata
	{
		public ContentTypeAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

	public interface IContentTypeRegistry
	{
		IContentType ForFileExtension(string fileExtension);
		IContentType ForName(string contentTypeName);
	}
}
