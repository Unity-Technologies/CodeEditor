using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Languages.UnityScript.ContentType;
using UnityScript.Parser;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.UnityScript
{
	[Export(typeof(ISourceSymbolProvider))]
	[ContentType(UnityScriptContentType.Name)]
	public class UnityScriptSourceSymbolProvider : ISourceSymbolProvider
	{
		public ISourceSymbol[] SourceSymbolsFor(IFile file)
		{
			var symbolCollector = new UnityScriptSymbolCollector(file);
			SyntaxTreeFor(file).Accept(symbolCollector);
			return symbolCollector.SourceSymbols;
		}

		static CompileUnit SyntaxTreeFor(IFile file)
		{
			var compileUnit = new CompileUnit();
			UnityScriptParser.ParseReader(file.OpenText(), "", new CompilerContext(), compileUnit);
			return compileUnit;
		}

		public class UnityScriptSymbolCollector : FastDepthFirstVisitor
		{
			readonly IFile _sourceFile;
			readonly List<ISourceSymbol> _symbols = new List<ISourceSymbol>();

			public UnityScriptSymbolCollector(IFile sourceFile)
			{
				_sourceFile = sourceFile;
			}

			public ISourceSymbol[] SourceSymbols
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
				_symbols.Add(new UnityScriptSourceSymbol(node, _sourceFile));
			}

			class UnityScriptSourceSymbol : ISourceSymbol
			{
				readonly TypeMember _node;
				readonly IFile _sourceFile;

				public UnityScriptSourceSymbol(TypeMember node, IFile sourceFile)
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