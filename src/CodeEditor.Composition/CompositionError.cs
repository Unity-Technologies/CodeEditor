using System;

namespace CodeEditor.Composition
{
	public class CompositionError
	{
		public CompositionError(Type contractType, string message)
		{
			ContractType = contractType;
			Message = message;
		}

		public Type ContractType { get; private set; }

		public string Message { get; private set; }
	}
}