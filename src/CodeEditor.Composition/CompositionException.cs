using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CodeEditor.Composition
{
	[Serializable]
	public class CompositionException : Exception
	{
		private readonly IList<CompositionError> _errors = new List<CompositionError>();

		public CompositionException(CompositionError error) : base(error.Message)
		{
			Add(error);
		}

		public CompositionException(Exception cause, CompositionError error) : base(error.Message, cause)
		{
			Add(error);
		}

		protected CompositionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public IList<CompositionError> Errors
		{
			get { return _errors; }
		}

		private void Add(CompositionError error)
		{
			_errors.Add(error);
		}
	}
}