using System;
using CodeEditor.Composition;

namespace CodeEditor.ContentTypes
{
	/// <summary>
	/// Provides a way for accessing content type specific services.
	/// </summary>
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
	/// Extension point for defining new content types.
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

	/// <summary>
	/// Provides a way for defining new content types.
	/// </summary>
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
		string ContentTypeName { get; }
	}

	/// <summary>
	/// Provides a way to associate a file extension to a content type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class FileExtensionToContentTypeAttribute : ExportAttribute, IFileExtensionToContentTypeMetadata
	{
		public FileExtensionToContentTypeAttribute(string contentType, string fileExtension) : base(typeof(IFileExtensionToContentTypeDefinition))
		{
			ContentTypeName = contentType;
			FileExtension = fileExtension;
		}

		public string FileExtension { get; private set; }
		public string ContentTypeName { get; private set; }
	}

	public interface IContentTypeMetadata
	{
		string Name { get; }
	}

	/// <summary>
	/// Associates an export with a content type by name.
	/// 
	/// <example>
	/// [ExportDefinition(typeof(ICompletionProviderFactory))]
	/// [ContentType(CSharpContentType.Name)]
	/// class CSharpCompletionProviderFactory : ICompletionProviderFactory { ... }
	/// </example>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ContentTypeAttribute : Attribute, IContentTypeMetadata
	{
		public ContentTypeAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}

	/// <summary>
	/// Provides a way to query available content types.
	/// </summary>
	public interface IContentTypeRegistry
	{
		IContentType ForFileExtension(string fileExtension);
		IContentType ForName(string contentTypeName);
	}

	public static class ContentTypeRegistryExtensions
	{
		/// <summary>
		/// Returns the content type specific implementation of service <typeparamref name="T"/>
		/// based on a file extension.
		/// </summary>
		/// <typeparam name="T">Service contract.</typeparam>
		/// <returns>Returns default(T) if either the content type or the service are not
		/// found.</returns>
		public static T ServiceForFileExtension<T>(this IContentTypeRegistry contentTypeRegistry, string fileExtension)
		{
			var contentType = contentTypeRegistry.ForFileExtension(fileExtension);
			return contentType != null
				? contentType.GetService<T>()
				: default(T);
		}
	}
}
