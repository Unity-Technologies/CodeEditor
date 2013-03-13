using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public interface ISourceSymbolProvider
	{
		ISourceSymbol[] SourceSymbolsFor(IFile file);
	}

	public interface ISourceSymbolProviderSelector : ISourceSymbolProvider
	{
	}
 
	[Export(typeof(ISourceSymbolProviderSelector))]
	public class SourceSymbolProviderSelector : ISourceSymbolProviderSelector
	{
		[Import]
		public IContentTypeRegistry ContentTypeRegistry { get; set; }

		public ISourceSymbol[] SourceSymbolsFor(IFile file)
		{
			var provider = ProviderFor(file);
			return provider != null
				? provider.SourceSymbolsFor(file)
				: new ISourceSymbol[0];
		}

		private ISourceSymbolProvider ProviderFor(IFile file)
		{
			return ContentTypeRegistry.ServiceForFileExtension<ISourceSymbolProvider>(file.Extension);
		}
	}
}