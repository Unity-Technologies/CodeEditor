namespace CodeEditor.Text.Data
{
	public interface ITextBuffer
	{
		IContentType ContentType { get; }
		ITextSnapshot CurrentSnapshot { get; }

		void Insert(int position, string text);
		void Delete(int start, int length);

		event TextChange Changed;
	}

	public static class TextBufferExtensions
	{
		public static void Append(this ITextBuffer buffer, string text)
		{
			buffer.Insert(buffer.CurrentSnapshot.Length, text);
		}
	}
}
