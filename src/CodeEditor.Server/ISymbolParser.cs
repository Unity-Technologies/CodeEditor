using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Server.Interface;

namespace CodeEditor.Server
{
	public interface ISymbolParser
	{
		ISymbol[] Parse(IFile file);
	}

	[Export(typeof(ISymbolParser))]
	class SymbolParser : ISymbolParser
	{
		public ISymbol[] Parse(IFile file)
		{
			return new ISymbol[] {new Symbol()};
		}
	}
}