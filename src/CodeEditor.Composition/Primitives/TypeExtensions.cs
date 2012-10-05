using System;
using System.Reflection;

namespace CodeEditor.Composition.Primitives
{
	static class TypeExtensions
	{
		private const BindingFlags InstanceMemberFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static MemberInfo[] InstanceMembers(this Type type)
		{
			return type.GetMembers(InstanceMemberFlags);
		}

		public static ConstructorInfo[] InstanceConstructors(this Type type)
		{
			return type.GetConstructors(InstanceMemberFlags);
		}
	}
}