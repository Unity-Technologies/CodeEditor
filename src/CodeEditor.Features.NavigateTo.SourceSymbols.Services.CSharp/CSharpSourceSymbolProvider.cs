using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Languages.CSharp.ContentType;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.CSharp
{
	[Export(typeof(ISourceSymbolProvider))]
	[ContentType(CSharpContentType.Name)]
	public class CSharpSourceSymbolProvider : ISourceSymbolProvider
	{
		public ISourceSymbol[] SourceSymbolsFor(IFile file)
		{
			var syntaxTree = SyntaxTreeFor(file);
			var symbolCollector = new CSharpSymbolCollector(file);
			syntaxTree.AcceptVisitor(symbolCollector);
			return symbolCollector.SourceSymbols;
		}

		static SyntaxTree SyntaxTreeFor(IFile file)
		{
			return new CSharpParser().Parse(file.ReadAllText());
		}

		class CSharpSymbolCollector : DepthFirstAstVisitor
		{
			readonly IFile _sourceFile;
			readonly List<ISourceSymbol> _symbols = new List<ISourceSymbol>();

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
				_symbols.Add(new CSharpSourceSymbol(entityDeclaration, _sourceFile));
			}

			public ISourceSymbol[] SourceSymbols
			{
				get { return _symbols.ToArray(); }
			}
		}

		class CSharpSourceSymbol : ISourceSymbol
		{
			readonly EntityDeclaration _declaration;
			readonly IFile _sourceFile;

			public CSharpSourceSymbol(EntityDeclaration declaration, IFile sourceFile)
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
				get { return StartLocation.Line; }
			}

			public int Column
			{
				get { return StartLocation.Column; }
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