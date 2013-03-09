using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;

namespace CodeEditor.Server
{
	public interface ISymbolParser
	{
		ISymbol[] Parse(IFile file);
	}

	public interface ISymbolParserSelector : ISymbolParser
	{
	}
 
	[Export(typeof(ISymbolParserSelector))]
	public class SymbolParserSelector : ISymbolParserSelector
	{
		[Import]
		public IContentTypeRegistry ContentTypeRegistry { get; set; }

		public ISymbol[] Parse(IFile file)
		{
			var symbolParser = SymbolParserFor(file);
			return symbolParser != null
				? symbolParser.Parse(file)
				: new ISymbol[0];
		}

		private ISymbolParser SymbolParserFor(IFile file)
		{
			return ContentTypeRegistry.ServiceForFileExtension<ISymbolParser>(file.Extension);
		}
	}
}