using System;

namespace CodeEditor.Composition.Primitives
{
	public class ImportDefinition
	{
		private readonly Action<Export[], object> _action;
		private readonly Func<Export, bool> _constraint;

		public ImportDefinition(Type contractType, ImportCardinality cardinality, Action<Export[], object> action, Func<Export, bool> constraint)
		{
			ContractType = contractType;
			Cardinality = cardinality;
			_action = action;
			_constraint = constraint;
		}

		public Type ContractType { get; private set; }

		public ImportCardinality Cardinality { get; private set; }

		public void SatisfyWith(Export[] export, object target)
		{
			_action(export, target);
		}

		public bool IsSatisfiableBy(Export export)
		{
			return _constraint(export);
		}
	}
}