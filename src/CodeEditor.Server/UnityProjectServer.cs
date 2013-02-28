using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Reactive;
using CodeEditor.Server.Interface;

namespace CodeEditor.Server
{
	[Export(typeof(IUnityProjectServer))]
	public class UnityProjectServer : MarshalByRefObject, IUnityProjectServer
	{
		[Import]
		public IFileSystem FileSystem { get; set; }

		[Import]
		public ISymbolParser SymbolParser { get; set; }

		public IUnityProject ProjectForFolder(string projectFolder)
		{
			var project = new UnityProject(FileSystem.FolderFor(projectFolder), SymbolParser);
			project.Start();
			return project;
		}
	}

	class UnityProject : MarshalByRefObject, IUnityProject
	{
		private readonly IFolder _projectFolder;
		private readonly ISymbolParser _parser;

		private IObservableX<ISymbol> AllSymbols;

		public UnityProject(IFolder projectFolder, ISymbolParser parser)
		{
			_projectFolder = projectFolder;
			_parser = parser;
		}

		public IObservableX<ISymbol> SearchSymbol(string pattern)
		{
			return AllSymbols
				.Where(symbol => symbol.DisplayText.Contains(pattern))
				.Do(_ => Console.Error.WriteLine("seen symbol {0}", _))
				.Remotable();
		}

		public void Start()
		{
			AllSymbols = SymbolsFromSourceFiles().Merge().SelectMany(_ => _);
		}

		private List<IObservableX<ISymbol[]>> SymbolsFromSourceFiles()
		{
			return SourceFiles.Select(file => ObservableSymbolsOf(file)).ToList();
		}

		protected IEnumerable<IFile> SourceFiles
		{
			get { return _projectFolder.GetFiles("*.cs", SearchOption.AllDirectories); }
		}

		private IObservableX<ISymbol[]> ObservableSymbolsOf(IFile file)
		{
			return ObservableX.Start(() => ParseSymbolsOf(file));
		}

		private ISymbol[] ParseSymbolsOf(IFile file)
		{
			Console.Error.WriteLine("Parsing {0}", file);
			return _parser.Parse(file);
		}
	}
}
