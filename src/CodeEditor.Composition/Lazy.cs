using System;

namespace CodeEditor.Composition
{
	/// <summary>
	/// A lazy import to allow selecting the right service based on
	/// available metadata.
	///
	/// The service is only requested from the container when <see cref="Lazy{T}.Value"/>
	/// is called.
	/// </summary>
	/// <typeparam name="T">the contract type</typeparam>
	/// <typeparam name="TMetadata">the metadata type (must be compatible with a metadata attribute)</typeparam>
	public class Lazy<T, TMetadata> : Lazy<T> where T: class
	{
		private readonly TMetadata _metadata;

		public Lazy(Func<T> valueFactory, TMetadata metadata) : base(valueFactory)
		{
			_metadata = metadata;
		}

		public Lazy(Func<object> valueFactory, TMetadata metadata) : base(() => (T)valueFactory())
		{
			_metadata = metadata;
		}

		public TMetadata Metadata
		{
			get { return _metadata; }
		}
	}

	public class Lazy<T> where T : class
	{
		private Func<T> _valueFactory;
		private bool _hasValue;
		private T _value;

		public Lazy(Func<T> valueFactory)
		{
			_valueFactory = valueFactory;
		}

		public T Value
		{
			get
			{
				lock (this)
				{
					if (!_hasValue)
					{
						_value = _valueFactory();
						_valueFactory = null;
						_hasValue = true;
					}
					return _value;
				}
			}
		}
	}
}
