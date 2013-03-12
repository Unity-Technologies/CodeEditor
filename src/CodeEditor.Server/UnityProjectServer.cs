using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using CodeEditor.ReactiveServiceStack;
using CodeEditor.Server.Interface;
using ServiceStack.ServiceInterface;
using IFile = CodeEditor.IO.IFile;

namespace CodeEditor.Server
{
	public class SymbolService : AsyncServiceBase<SymbolSearch>
	{
		public IUnityProjectProvider ProjectProvider { get; set; }

		protected override object Run(SymbolSearch request)
		{
			return
				ProjectProvider
				.Project
				.SearchSymbol(request.Filter)
				.Select(s => new Symbol {DisplayText = s.DisplayText, SourceFile = RelativePathFor(s.SourceFile), Line = s.Line, Column = s.Column})
				.ToJsonStreamWriter();
		}

		string RelativePathFor(IFile sourceFile)
		{
			return sourceFile.FullName;
		}
	}

	public interface IUnityProjectProvider
	{
		IUnityProject Project { get; }
	}

	[Export(typeof(IUnityProjectProvider))]
	class UnityProjectProvider : IUnityProjectProvider
	{
		[Import]
		public IUnityAssetsFolderProvider AssetsFolderProvider { get; set; }

		[Import]
		public ISymbolParser SymbolParser { get; set; }

		public IUnityProject Project
		{
			get { return _project.Value; }
		}

		readonly Lazy<IUnityProject> _project;

		public UnityProjectProvider()
		{
			 _project = new Lazy<IUnityProject>(CreateProject); 
		}

		IUnityProject CreateProject()
		{
			var project = new UnityProject(AssetsFolderProvider.AssetsFolder, SymbolParser);
			project.Start();
			return project;
		}
	}

	public interface IUnityAssetsFolderProvider
	{
		IFolder AssetsFolder { get; }
	}

	[Export(typeof(IUnityAssetsFolderProvider))]
	class ServerAssetsFolderProvider : IUnityAssetsFolderProvider
	{
		[Import]
		public IFileSystem FileSystem { get; set; }

		[Import]
		public ILogger Logger { get; set; } 

		public IFolder AssetsFolder
		{
			get
			{
				// ServerDirectory is $Project/Library/CodeEditor/Server
				var assetsFolder = Path.Combine(ServerDirectory, "../../../Assets");
				Logger.Log("Assets folder is " + assetsFolder);
				return FileSystem.FolderFor(assetsFolder);
			}
		}

		private string ServerDirectory
		{
			get { return Path.GetDirectoryName(GetType().Module.FullyQualifiedName); }
		}
	}

	public interface IUnityProject
	{
		IObservableX<ISymbol> SearchSymbol(string pattern);
	}

	public interface ISymbol
	{
		IFile SourceFile { get; }
		int Line { get; }
		int Column { get; }
		string DisplayText { get; }
	}

	class UnityProject : IUnityProject
	{
		private readonly IFolder _assetsFolder;
		private readonly ISymbolParser _parser;

		private IObservableX<ISymbol> AllSymbols;

		public UnityProject(IFolder assetsFolder, ISymbolParser parser)
		{
			_assetsFolder = assetsFolder;
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
			get { return _assetsFolder.GetFiles("*.cs", SearchOption.AllDirectories); }
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
