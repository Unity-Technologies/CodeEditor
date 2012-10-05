using System;

namespace CodeEditor.Composition
{
	/// <summary>
	/// Marks a class as an export.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ExportAttribute : ContractAttribute
	{
		public ExportAttribute(Type contractType) : base(contractType) {}

		public ExportAttribute() : base(null) {}
	}
}