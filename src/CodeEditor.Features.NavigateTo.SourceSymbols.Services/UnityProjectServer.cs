using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
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

		string ServerDirectory
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

		/// <summary>
		/// 1 based.
		/// </summary>
		int Line { get; }

		/// <summary>
		/// 1 based.
		/// </summary>
		int Column { get; }

		string DisplayText { get; }
	}

	class UnityProject : IUnityProject
	{
		readonly IFolder _assetsFolder;
		readonly ISymbolParser _parser;

		IObservableX<ISymbol> _allSymbols;

		public UnityProject(IFolder assetsFolder, ISymbolParser parser)
		{
			_assetsFolder = assetsFolder;
			_parser = parser;
		}

		public IObservableX<ISymbol> SearchSymbol(string pattern)
		{
			if (string.IsNullOrEmpty(pattern))
				return _allSymbols;
			return _allSymbols
				.Where(symbol => symbol.DisplayText.Contains(pattern));
		}

		public void Start()
		{
			_allSymbols = SymbolsFromSourceFiles().Merge().SelectMany(_ => _);
		}

		List<IObservableX<ISymbol[]>> SymbolsFromSourceFiles()
		{
			return SourceFiles.Select(file => ObservableSymbolsOf(file)).ToList();
		}

		protected IEnumerable<IFile> SourceFiles
		{
			get { return _assetsFolder.GetFiles("*.cs", SearchOption.AllDirectories); }
		}

		IObservableX<ISymbol[]> ObservableSymbolsOf(IFile file)
		{
			return ObservableX.Start(() => ParseSymbolsOf(file));
		}

		ISymbol[] ParseSymbolsOf(IFile file)
		{
			return _parser.Parse(file);
		}
	}
}