using System;

namespace CodeEditor.Reactive.Disposables
{
	public class MultipleAssignmentDisposable : IDisposable
	{
		readonly object _lock = new object();
		bool _disposed;
		IDisposable _disposable;

		public IDisposable Disposable
		{
			get
			{
				lock (_lock)
					return _disposable;
			}
			set
			{
				lock (_lock)
				{
					if (_disposed)
						value.Dispose();
					else
						_disposable = value;
				}
			}
		}

		public void Dispose()
		{
			lock (_lock)
			{
				if (_disposed)
					return;
				_disposed = true;
				if (_disposable == null)
					return;
				_disposable.Dispose();
				_disposable = null;
			}
		}
	}
}
