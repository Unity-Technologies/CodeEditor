using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public static class MissingEngineAPI
	{
		public static GUIContent GUIContent_Temp(string content)
		{
			return _GUIContent_Temp(content);
		}

		private static readonly GUIContent_Temp_T _GUIContent_Temp = (GUIContent_Temp_T)DelegateForStaticMethodOf<GUIContent, GUIContent_Temp_T>("Temp");

		private delegate GUIContent GUIContent_Temp_T(string content);

		private static Delegate DelegateForStaticMethodOf<T, TDelegate>(string name)
		{
			return DelegateForStaticMethodOf(typeof(T), name, typeof(TDelegate));
		}

		private static Delegate DelegateForStaticMethodOf(Type type, string name, Type delegateType)
		{
			return Delegate.CreateDelegate(delegateType, StaticMethodOf(type, name, DelegateSignature(delegateType)));
		}

		private static Type[] DelegateSignature(Type delegateType)
		{
			return delegateType.GetMethod("Invoke").GetParameters().Select(_ => _.ParameterType).ToArray();
		}

		private static MethodInfo StaticMethodOf(Type type, string name, params Type[] signature)
		{
			var result = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, signature, null);
			if (result == null)
				throw new ArgumentException(string.Format("Can't find method {0}.{1}({2})", type.Name, name, string.Join(", ", signature.Select(_ => _.FullName).ToArray())));
			return result;
		}
	}
}
