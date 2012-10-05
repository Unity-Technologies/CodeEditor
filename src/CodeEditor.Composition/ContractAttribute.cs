using System;

namespace CodeEditor.Composition
{
	public abstract class ContractAttribute : Attribute
	{
		private readonly Type _contractType;

		protected ContractAttribute(Type contractType)
		{
			_contractType = contractType;
		}

		public Type ContractType
		{
			get { return _contractType; }
		}
	}
}