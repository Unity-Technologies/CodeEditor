using System;

namespace CodeEditor.Reactive.Disposables
{
	public static class Disposable
	{
		public static IDisposable Create(Action dispose)
		{
			return System.Disposables.Disposable.Create(dispose);
		}
	}
}