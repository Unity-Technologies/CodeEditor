using System;
using System.Reflection;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	static class MissingEditorAPI
	{
		public static bool ParentHasFocus(EditorWindow editorWindow)
		{
			return (bool) PropertyOf(ParentOf(editorWindow), "hasFocus");
		}

		private static object ParentOf(EditorWindow editorWindow)
		{
			return FieldOf(editorWindow, "m_Parent");
		}

		private static object PropertyOf(object o, string propertyName)
		{
			var property = o.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
				throw new ArgumentException(string.Format("Can't find {0}.{1}", o, propertyName));
			return property.GetValue(o, null);
		}

		private static object FieldOf(object o, string fieldName)
		{
			var type = o.GetType();
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field == null)
				throw new ArgumentException(string.Format("Can't find {0}.{1}", o, fieldName));
			return field.GetValue(o);
		}
	}
}
