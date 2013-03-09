using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Languages.CSharp;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace CodeEditor.Server.CSharp
{
	[Export(typeof(ISymbolParser))]
	[ContentType(CSharpContentType.Name)]
	public class CSharpSymbolParser : ISymbolParser
	{
		public ISymbol[] Parse(IFile file)
		{
			var syntaxTree = SyntaxTreeFor(file);
			var symbolCollector = new CSharpSymbolCollector();
			syntaxTree.AcceptVisitor(symbolCollector);
			return symbolCollector.Symbols;
		}

		private static SyntaxTree SyntaxTreeFor(IFile file)
		{
			return new CSharpParser().Parse(file.ReadAllText());
		}

		class CSharpSymbolCollector : DepthFirstAstVisitor
		{
			private readonly List<ISymbol> _symbols = new List<ISymbol>();

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				AddSymbolFor(typeDeclaration);
			}

			private void AddSymbolFor(EntityDeclaration entityDeclaration)
			{
				_symbols.Add(new CSharpSymbol(entityDeclaration));
			}

			public ISymbol[] Symbols
			{
				get { return _symbols.ToArray(); }
			}
		}

		class CSharpSymbol : ISymbol
		{
			private readonly EntityDeclaration _declaration;

			public CSharpSymbol(EntityDeclaration declaration)
			{
				_declaration = declaration;
			}

			public int Line
			{
				get { return StartLocation.Line; }
			}

			public int Column
			{
				get { return StartLocation.Column; }
			}

			private TextLocation StartLocation
			{
				get { return _declaration.NameToken.StartLocation; }
			}

			public string DisplayText
			{
				get { return _declaration.Name; }
			}
		}
	}
}
