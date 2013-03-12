using System;

namespace CodeEditor.Composition
{
	/// <summary>
	/// Marks a class as an export.
	///
	/// Subclasses are not considered exports, if you want to have
	/// all subclasses automatically considered as exports use
	/// <see cref="InheritedExportAttribute"/>.
	///
	/// The contract type defaults to the type marked with the attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class ExportAttribute : ContractAttribute
	{
		public ExportAttribute(Type contractType) : base(contractType) {}

		public ExportAttribute() : base(null) {}
	}

	/// <summary>
	/// Marks a class as an inherited export.
	///
	/// Subclasses are automatically considered exports.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class InheritedExportAttribute : ExportAttribute
	{
		public InheritedExportAttribute(Type contractType) : base(contractType) {}

		public InheritedExportAttribute() : base(null) {}
	}
}