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
			var symbolCollector = new CSharpSymbolCollector(file);
			syntaxTree.AcceptVisitor(symbolCollector);
			return symbolCollector.Symbols;
		}

		static SyntaxTree SyntaxTreeFor(IFile file)
		{
			return new CSharpParser().Parse(file.ReadAllText());
		}

		class CSharpSymbolCollector : DepthFirstAstVisitor
		{
			readonly IFile _sourceFile;
			readonly List<ISymbol> _symbols = new List<ISymbol>();

			public CSharpSymbolCollector(IFile sourceFile)
			{
				_sourceFile = sourceFile;
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				AddSymbolFor(typeDeclaration);
			}

			void AddSymbolFor(EntityDeclaration entityDeclaration)
			{
				_symbols.Add(new CSharpSymbol(entityDeclaration, _sourceFile));
			}

			public ISymbol[] Symbols
			{
				get { return _symbols.ToArray(); }
			}
		}

		class CSharpSymbol : ISymbol
		{
			readonly EntityDeclaration _declaration;
			readonly IFile _sourceFile;

			public CSharpSymbol(EntityDeclaration declaration, IFile sourceFile)
			{
				_declaration = declaration;
				_sourceFile = sourceFile;
			}

			public IFile SourceFile
			{
				get { return _sourceFile; }
			}

			public int Line
			{
				get { return StartLocation.Line - 1; }
			}

			public int Column
			{
				get { return StartLocation.Column - 1; }
			}

			TextLocation StartLocation
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