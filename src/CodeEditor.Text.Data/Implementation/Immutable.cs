using System;

namespace CodeEditor.Text.Data.Implementation
{
	public struct Immutable<T>
	{
		private T _value;
		private bool _hasValue;

		public T Value
		{
			get
			{
				if (!_hasValue)
					throw new InvalidOperationException("Value hasn't been set yet");
				return _value;
			}

			set
			{
				if (_hasValue)
					throw new InvalidOperationException("Value has already been set");
				_value = value;
				_hasValue = true;
			}
		}
	}
}