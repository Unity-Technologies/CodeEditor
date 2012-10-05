using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CodeEditor.Composition.Hosting
{
	static class AssemblyExtensions
	{
		public static bool IsSafeForComposition(this Assembly assembly)
		{
			return CaresForComposition(assembly) && TypesAreAccessible(assembly);
		}

		private static bool CaresForComposition(Assembly assembly)
		{
			return assembly
				.GetReferencedAssemblies()
				.Any(name => name.FullName.Equals(typeof(ExportAttribute).Assembly.FullName));
		}

		private static bool TypesAreAccessible(Assembly assembly)
		{
			try
			{
				assembly.GetTypes();
				return true;
			}
			catch (Exception e)
			{
				Trace.Write(string.Format("Error loading types from assembly `{0}': {1}", assembly, e));
				return false;
			}
		}
	}
}