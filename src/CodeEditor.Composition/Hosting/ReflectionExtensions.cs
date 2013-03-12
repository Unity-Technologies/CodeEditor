using System;
using System.Collections.Generic;

namespace CodeEditor.Composition.Hosting
{
	static class ReflectionExtensions
	{
		public static IEnumerable<Type> BaseTypes(this Type type)
		{
			var baseType = type.BaseType;
			while (baseType != null)
			{
				yield return baseType;
				baseType = baseType.BaseType;
			}
		}
	}
}