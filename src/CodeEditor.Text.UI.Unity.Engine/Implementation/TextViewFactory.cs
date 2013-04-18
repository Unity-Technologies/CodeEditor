using System;
using CodeEditor.Composition;
using CodeEditor.IO;

/*
 Notes:
 * Considerations when adding new Imports:
 *	- Is the component stateless? if so it can be reused in all CreateView calls (can be a singleton)
 *	- If not then create a Provider that creates a new instance for the textview
 *	- E.g Settings live per textview (reads/writes from/to global prefs but once in memory it has its own state -> this way we can have multiple CodeEditorWindows)
 *  - All components that uses settings (state) needs to be owned by textview and should have a Provider
 */

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	[Export(typeof(ITextViewFactory))]
	class TextViewFactory : ITextViewFactory
	{
		[Import]
		ITextViewDocumentFactory DocumentFactory { get; set; }

		[Import]
		ITextViewAppearanceProvider AppearanceProvider { get; set; }
		
		[Import]
		IFontManagerProvider FontManagerProvider { get; set; }

		[Import]
		IDefaultTextViewMarginsProvider DefaultTextViewMarginsProvider { get; set; }

		[Import]
		ITextViewWhitespaceProvider WhitespaceProvider { get; set; }

		[Import]
		ISettingsProvider SettingsProvider { get; set; }

		[Import]
		IPreferences Preferences { get; set; }

		[Import]
		IFileSystem FileSystem { get; set; }

		[Import]
		ITextViewAdornments Adornments { get; set; }

		[Import]
		IMouseCursorRegions MouseCursorRegions { get; set; }

		[Import]
		IMouseCursors MouseCursors { get; set; }

		public ITextView ViewForFile(string fileName)
		{
			return CreateView(new TextViewCreationOptions {File = FileSystem.GetFile(fileName)});
		}

		public ITextView CreateView()
		{
			return CreateView(new TextViewCreationOptions());
		}

		public ITextView CreateView(TextViewCreationOptions options)
		{
			var file = options.File ?? TransientTextFile();
			var document = DocumentFor(file);
			var settings = SettingsProvider.GetSettings(Preferences);
			var fontManager = FontManagerProvider.GetFontManager(settings);
			var textView = new TextView(document, AppearanceFor(document, fontManager), Adornments, MouseCursors, MouseCursorRegions, WhiteSpace(settings), settings, fontManager);
			textView.Margins = options.Margins ?? DefaultMarginsFor(textView);
			return textView;
		}

		private static TransientFile TransientTextFile()
		{
			return new TransientFile(".txt");
		}

		private ITextViewDocument DocumentFor(IFile file)
		{
			return DocumentFactory.DocumentForFile(file);
		}

		private ITextViewWhitespace WhiteSpace(ISettings settings)
		{
			return WhitespaceProvider.GetWhitespace(settings);
		}

		private ITextViewAppearance AppearanceFor(ITextViewDocument document, IFontManager fontManager)
		{
			return AppearanceProvider.AppearanceFor(document, fontManager);
		}

		private ITextViewMargins DefaultMarginsFor(ITextView textView)
		{
			return DefaultTextViewMarginsProvider.MarginsFor(textView);
		}

		class TransientFile : IFile
		{
			readonly ResourcePath _path;

			public TransientFile(string extension)
			{
				_path = new ResourcePath(extension);
			}

			public ResourcePath Path
			{
				get { return _path; }
			}

			public string Location
			{
				get { return ""; }
			}

			public string Extension
			{
				get { return _path.Extension; }
			}

			public string ReadAllText()
			{
				return "";
			}

			public void WriteAllText(string text)
			{
				throw new InvalidOperationException();
			}

			public void Delete()
			{
				throw new InvalidOperationException();
			}

			public bool Exists()
			{
				return false;
			}
		}
	}
}
