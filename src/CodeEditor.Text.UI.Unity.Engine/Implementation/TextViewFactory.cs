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
		ITextViewMarginsFactory TextViewMarginsFactory { get; set; }


		public ITextView ViewForFile(string fileName)
		{
			return ForFile(FileSystem.FileFor(fileName));
		}

		public ITextView CreateView()
		{
			return ForFile(new TransientFile(".txt"));
		}

		private ITextView ForFile(IFile file)
		{
			var document = DocumentFactory.DocumentForFile(file);
			var textView = new TextView(document, AppearanceProvider.AppearanceFor(document), Adornments);
			textView.Margins = MarginsFor(textView);
			return textView;
		}

		private ITextViewMargins MarginsFor(TextView textView)
		{
			return TextViewMarginsFactory.MarginsFor(textView);
		}

		class TransientFile : IFile
		{
			private readonly string _extension;

			public TransientFile(string extension)
			{
				_extension = extension;
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
		}
	}
}
