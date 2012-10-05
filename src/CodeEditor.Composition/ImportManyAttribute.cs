using System;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Composition
{
	/// <summary>
	/// Imports all providers of a service.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ImportManyAttribute : ImportAttribute
	{
		public ImportManyAttribute(Type contractType) : base(contractType) { }
		public ImportManyAttribute() : base(null) { }

		public override ImportCardinality Cardinality
		{
			get { return ImportCardinality.Many; }
		}
	}
}