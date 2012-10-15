using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public static class CodeTimer
	{
		public static T LoggingTime<T>(string label, Func<T> func)
		{
			var watch = Stopwatch.StartNew();
			try
			{
				return func();
			}
			finally
			{
				watch.Stop();
				Debug.Log(label + " took " + watch.ElapsedMilliseconds + " ms.");
			}
		}
	}
}
