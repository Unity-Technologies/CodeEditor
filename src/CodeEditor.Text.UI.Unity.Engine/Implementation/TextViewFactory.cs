using System;
using CodeEditor.Composition;
using CodeEditor.IO;

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
		IFileSystem FileSystem { get; set; }

		[Import]
		ITextViewAdornments Adornments { get; set; }

		[Import]
		IDefaultTextViewMarginsProvider DefaultTextViewMarginsProvider { get; set; }

		[Import]
		IMouseCursorRegions MouseCursorRegions { get; set; }

		[Import]
		IMouseCursors MouseCursors { get; set; }

		public ITextView ViewForFile(string fileName)
		{
			return CreateView(new TextViewCreationOptions {File = FileSystem.FileFor(fileName)});
		}

		public ITextView CreateView()
		{
			return CreateView(new TextViewCreationOptions());
		}

		public ITextView CreateView(TextViewCreationOptions options)
		{
			var file = options.File ?? TransientTextFile();
			var document = DocumentFor(file);
			var textView = new TextView(document, AppearanceFor(document), Adornments, MouseCursors, MouseCursorRegions);
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

		private ITextViewAppearance AppearanceFor(ITextViewDocument document)
		{
			return AppearanceProvider.AppearanceFor(document);
		}

		private ITextViewMargins DefaultMarginsFor(ITextView textView)
		{
			return DefaultTextViewMarginsProvider.MarginsFor(textView);
		}

		class TransientFile : IFile
		{
			private readonly string _extension;

			public TransientFile(string extension)
			{
				_extension = extension;
			}

			public string FullName
			{
				get { return ""; }
			}

			public string Extension
			{
				get { return _extension; }
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
		}
	}
}
