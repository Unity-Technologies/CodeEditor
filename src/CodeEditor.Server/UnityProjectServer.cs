using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Reactive;
using CodeEditor.Server.Interface;
using ServiceStack.ServiceInterface;
using IFile = CodeEditor.IO.IFile;

namespace CodeEditor.Server
{
	public class SymbolService : AsyncServiceBase<SymbolSearch>
	{
		public IUnityProjectFactory ProjectFactory { get; set; }

		protected override object Run(SymbolSearch request)
		{
			return
				Symbols
				.Where(_ => _.Contains(request.Filter))
				.Select(s => new Interface.Symbol {DisplayText = s, Line = 1, Column = 42})
				.ToObservableX()
				.ToJsonStreamWriter();
		}

		private static string[] Symbols = new[] {"Foo.Update", "Foo.Start", "Bar.Start"};
	}

	public interface IUnityProjectFactory
	{
		IUnityProject ProjectForFolder(string projectFolder);
	}

	public interface IUnityProject
	{
		IObservableX<ISymbol> SearchSymbol(string pattern);
	}

	public interface ISymbol
	{
		int Line { get; }
		int Column { get; }
		string DisplayText { get; }
	}

	public class Symbol : ISymbol
	{
		public int Line
		{
			get { return 1; }
		}

		public int Column
		{
			get { return 7; }
		}

		public string DisplayText
		{
			get { return "Foo"; }
		}
	}

	[Export(typeof(IUnityProjectFactory))]
	public class UnityProjectFactory : IUnityProjectFactory
	{
		[Import]
		public IFileSystem FileSystem { get; set; }

		[Import]
		public ISymbolParser SymbolParser { get; set; }

		public IUnityProject ProjectForFolder(string projectFolder)
		{
			return _projects.GetOrAdd(Path.GetFullPath(projectFolder), CreateProjectForFolder);
		}

		private IUnityProject CreateProjectForFolder(string projectFolder)
		{
			var project = new UnityProject(FileSystem.FolderFor(projectFolder), SymbolParser);
			project.Start();
			return project;
		}

		readonly ConcurrentDictionary<string, IUnityProject> _projects = new ConcurrentDictionary<string, IUnityProject>();
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
			if (string.IsNullOrEmpty(pattern))
				return AllSymbols;
			return AllSymbols
				.Where(symbol => symbol.DisplayText.Contains(pattern));
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
			return _parser.Parse(file);
		}
	}
}
