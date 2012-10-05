using System;
using System.Collections.Generic;

namespace CodeEditor.Composition
{
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