using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using CodeEditor.IO;
using UnityScript.Parser;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.UnityScript
{
	public class UnityScriptSymbolParser : ISymbolParser
	{
		public ISymbol[] Parse(IFile file)
		{
			var symbolCollector = new UnityScriptSymbolCollector(file);
			ParseModule(file).Accept(symbolCollector);
			return symbolCollector.Symbols;
		}

		static CompileUnit ParseModule(IFile file)
		{
			var compileUnit = new CompileUnit();
			UnityScriptParser.ParseReader(file.OpenText(), "", new CompilerContext(), compileUnit);
			return compileUnit;
		}

		public class UnityScriptSymbolCollector : FastDepthFirstVisitor
		{
			readonly IFile _sourceFile;
			readonly List<ISymbol> _symbols = new List<ISymbol>();

			public UnityScriptSymbolCollector(IFile sourceFile)
			{
				_sourceFile = sourceFile;
			}

			public ISymbol[] Symbols
			{
				get { return _symbols.ToArray(); }
			}

			public override void OnClassDefinition(ClassDefinition node)
			{
				AddSymbolFor(node);
				base.OnClassDefinition(node);
			}

			void AddSymbolFor(TypeMember node)
			{
				_symbols.Add(new AstSymbol(node, _sourceFile));
			}

			public override void OnMethod(Method node)
			{
				base.OnMethod(node);
			}

			class AstSymbol : ISymbol
			{
				readonly TypeMember _node;
				readonly IFile _sourceFile;

				public AstSymbol(TypeMember node, IFile sourceFile)
				{
					_node = node;
					_sourceFile = sourceFile;
				}

				public IFile SourceFile
				{
					get { return _sourceFile; }
				}

				public int Line
				{
					get { return Location.Line; }
				}

				public int Column
				{
					get { return Location.Column; }
				}

				LexicalInfo Location
				{
					get { return _node.LexicalInfo; }
				}

				public string DisplayText
				{
					get { return _node.Name; }
				}
			}
		}
	}
}