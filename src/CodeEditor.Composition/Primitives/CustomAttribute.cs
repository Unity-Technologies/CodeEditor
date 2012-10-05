using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeEditor.Composition.Primitives
{
	static class CustomAttribute<T> where T : Attribute
	{
		public static T From(MemberInfo member)
		{
			return (T)Attribute.GetCustomAttribute(member, typeof(T));
		}

		public static IEnumerable<T> AllFrom(MemberInfo member)
		{
			return Attribute.GetCustomAttributes(member, typeof(T)).Cast<T>();
		}
	}
}