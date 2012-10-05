using System;

namespace CodeEditor.Composition
{
	[AttributeUsage(AttributeTargets.Constructor)]
	public class ImportingConstructor : Attribute
	{
	}
}