using CodeEditor.ContentTypes;
using CodeEditor.IO;

namespace CodeEditor.Text.Data.Implementation
{
	internal class TextDocument : ITextDocument
	{
		private readonly IFile _file;
		private readonly ITextBuffer _buffer;

		public TextDocument(IFile file, IContentType contentType)
		{
			_file = file;
			_buffer = new TextBuffer(file.ReadAllText(), contentType);
		}

		public IFile File
		{
			get { return _file; }
		}

		public ITextBuffer Buffer
		{
			get { return _buffer; }
		}

		public void Save()
		{
			_file.WriteAllText(_buffer.CurrentSnapshot.Text);
		}
	}
}
