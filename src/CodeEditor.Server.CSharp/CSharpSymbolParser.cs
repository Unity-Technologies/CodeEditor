using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Languages.CSharp;
using CodeEditor.Text.Data;

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
