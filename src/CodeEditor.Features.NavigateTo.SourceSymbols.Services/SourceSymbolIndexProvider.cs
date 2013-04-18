using System.IO;
using CodeEditor.Composition;
using CodeEditor.IO;
using CodeEditor.Logging;
using CodeEditor.Reactive;
using ServiceStack.Net30.Collections.Concurrent;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public interface ISourceSymbolIndexProvider
	{
		ISourceSymbolIndex Index { get; }
	}

	public interface ISourceSymbolIndex
	{
		IObservableX<ISourceSymbol> SearchSymbol(string filter);
	}

	public interface ISourceSymbol
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
		readonly ISourceSymbolProviderSelector _symbolProviderSelector;
		readonly IObservableX<IFileNotification> _sourceFileNotifications;
		readonly ConcurrentDictionary<IFile, ISourceSymbol[]> _sourceFileIndices = new ConcurrentDictionary<IFile, ISourceSymbol[]>();
		readonly ILogger _logger;

		public SourceSymbolIndex(ISourceSymbolProviderSelector symbolProviderSelector, IObservableX<IFileNotification> sourceFileNotifications, ILogger logger)
		{
			_sourceFileNotifications = sourceFileNotifications;
			_logger = logger;
			_symbolProviderSelector = symbolProviderSelector;
		}

		public IObservableX<ISourceSymbol> SearchSymbol(string filter)
		{
			return string.IsNullOrEmpty(filter)
				? AllSymbols
				: AllSymbols.Where(symbol => symbol.DisplayText.Contains(filter));
		}

		IObservableX<ISourceSymbol> AllSymbols
		{
			get { return _sourceFileIndices.Values.ToObservableX().SelectMany(_ => _); }
		}

		public void Start()
		{
			_sourceFileNotifications
				.ObserveOnThreadPool()
				.Subscribe(OnSourceFileNotification);
		}

		void OnSourceFileNotification(IFileNotification notification)
		{
			Log(notification);
			_sourceFileIndices.AddOrUpdate(notification.File, SourceSymbolsFor, (file, oldValue) => SourceSymbolsFor(file));
		}

		ISourceSymbol[] SourceSymbolsFor(IFile file)
		{
			return _symbolProviderSelector.SourceSymbolsFor(file);
		}

		void Log(object o)
		{
			_logger.Log(o);
		}
	}

	[Export(typeof(ISourceSymbolIndexProvider))]
	class SourceSymbolIndexProvider : ISourceSymbolIndexProvider
	{
		[Import]
		public ISourceFilesProvider SourceFilesProvider { get; set; }

		[Import]
		public ISourceSymbolProviderSelector SymbolProviderSelector { get; set; }

		[Import]
		public ILogger Logger { get; set; }

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
			var project = new SourceSymbolIndex(SymbolProviderSelector, SourceFilesProvider.SourceFiles, Logger);
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
				var assetsFolder = ServerDirectory.Combine("../../../Assets").ToAbsolutePath();
				Logger.Log("Assets folder is " + assetsFolder);
				return FileSystem.GetFolder(assetsFolder);
			}
		}

		ResourcePath ServerDirectory
		{
			get { return Path.GetDirectoryName(GetType().Module.FullyQualifiedName); }
		}
	}
}