using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.ContentTypes;
using CodeEditor.IO;
using CodeEditor.Reactive;
using CodeEditor.Reactive.Disposables;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public interface ISourceFilesProvider
	{
		IObservableX<IFileNotification> SourceFiles { get; }
	}

	public interface IFileNotification
	{
		IFile File { get; }
		FileNotificationKind NotificationKind { get; }
	}

	public enum FileNotificationKind
	{
		New
	}

	[Export(typeof(ISourceFilesProvider))]
	public class SourceFilesProvider : ISourceFilesProvider
	{
		[Import]
		public IContentTypeRegistry ContentTypeRegistry { get; set; }

		[Import]
		public IUnityAssetsFolderProvider SourceFolderProvider { get; set; }

		public IObservableX<IFileNotification> SourceFiles
		{
			get
			{
				return ObservableX.CreateWithDisposable<IFileNotification>(observer =>
				{
					foreach (var fileExtension in FileExtensionsWithExportedSourceSymbolProvider())
						foreach (var file in SourceFolder.SearchFiles("*" + fileExtension, SearchOption.AllDirectories))
							observer.OnNext(new FileNotification(file, FileNotificationKind.New));
					return Disposable.Empty;
				});
			}
		}

		IFolder SourceFolder
		{
			get { return SourceFolderProvider.AssetsFolder; }
		}

		IEnumerable<string> FileExtensionsWithExportedSourceSymbolProvider()
		{
			return ContentTypeRegistry
				.ContentTypes
				.Where(_ => _.HasService<ISourceSymbolProvider>())
				.SelectMany(_ => ContentTypeRegistry.FileExtensionsFor(_));
		}
	}

	class FileNotification : IFileNotification
	{
		public FileNotification(IFile file, FileNotificationKind notificationKind)
		{
			File = file;
			NotificationKind = notificationKind;
		}

		public IFile File { get; private set; }
		public FileNotificationKind NotificationKind { get; private set; }

		public override string ToString()
		{
			return string.Format("{0}:{1}", NotificationKind, File.Path);
		}
	}
}
