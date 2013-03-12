using System;

namespace CodeEditor.Composition
{
	/// <summary>
	/// Marks an assembly as providing exports. The attribute provides a way
	/// for clients to force a reference to the composition assembly and thus
	/// take part in composition even when they only reference subclasses
	/// of the <see cref="ExportAttribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class HasExportsAttribute : Attribute
	{
	}
}