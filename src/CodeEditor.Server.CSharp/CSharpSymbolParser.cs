using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Languages.CSharp;

namespace CodeEditor.Server.CSharp
{
	[Export(typeof(ISymbolParser))]
	[ContentType(CSharpContentType.Name)]
	public class CSharpSymbolParser : ISymbolParser
	{
		public ISymbol[] Parse(IFile file)
		{
			throw new System.NotImplementedException();
		}
	}
}
