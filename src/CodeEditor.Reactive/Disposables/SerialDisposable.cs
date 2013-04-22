using System;
using System.Disposables;

namespace CodeEditor.Reactive.Disposables
{
	public class SerialDisposable : IDisposable
	{
		readonly MutableDisposable _disposable = new MutableDisposable();

		public IDisposable Disposable
		{
			get { return _disposable.Disposable; }
			set { _disposable.Disposable = value; }
		}

		public void Dispose()
		{
			_disposable.Dispose();
		}
	}
}