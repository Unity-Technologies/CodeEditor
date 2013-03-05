using System;
using CodeEditor.Composition;

namespace CodeEditor.Logging
{
	public interface ILogger
	{
		void Log(object value);
		void LogError(Exception exception);
	}

	[Export(typeof(ILogger))]
	public class StandardLogger : ILogger
	{
		public void Log(object value)
		{
			Console.WriteLine(value);
		}

		public void LogError(Exception exception)
		{
			Console.Error.WriteLine(exception);
		}
	}
}