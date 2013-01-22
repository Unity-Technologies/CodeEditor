using System;

namespace CodeEditor.Composition
{
	[Serializable]
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