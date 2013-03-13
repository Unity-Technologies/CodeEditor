using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public interface ISourceSymbolIndex
	{
		IObservableX<ISymbol> SearchSymbol(string pattern);
	}

	public interface ISourceSymbolIndexProvider
	{
		ISourceSymbolIndex Index { get; }
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

	class SourceSymbolIndex : ISourceSymbolIndex
	{
		readonly IFolder _assetsFolder;
		readonly ISymbolParser _parser;

		IObservableX<ISymbol> _allSymbols;

		public SourceSymbolIndex(IFolder assetsFolder, ISymbolParserSelector parser)
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

	[Export(typeof(ISourceSymbolIndexProvider))]
	class SourceSymbolIndexProvider : ISourceSymbolIndexProvider
	{
		[Import]
		public IUnityAssetsFolderProvider AssetsFolderProvider { get; set; }

		[Import]
		public ISymbolParserSelector SymbolParser { get; set; }

		public ISourceSymbolIndex Index
		{
			get { return _index.Value; }
		}

		readonly Lazy<ISourceSymbolIndex> _index;

		public SourceSymbolIndexProvider()
		{
			_index = new Lazy<ISourceSymbolIndex>(CreateIndex);
		}

		ISourceSymbolIndex CreateIndex()
		{
			var project = new SourceSymbolIndex(AssetsFolderProvider.AssetsFolder, SymbolParser);
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
}