using System;
using UnityEngine;

namespace CodeEditor.Logging.Unity
{
	public class UnityLogger : ILogger
	{
		public void Log(object value)
		{
			Debug.Log(value);
		}

		public void LogError(Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}