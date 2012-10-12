using System;

namespace CodeEditor.Collections
{
	public static class ArrayExtensions
	{
		public static T[] Append<T>(this T[] array, T newElement)
		{
			var result = new T[array.Length + 1];
			Array.Copy(array, result, array.Length);
			result[array.Length] = newElement;
			return result;
		}

		public static T[] Replace<T>(this T[] array, int index, T newElement)
		{
			return array.ReplaceRange(index, 1, newElement);
		}

		public static T[] ReplaceRange<T>(this T[] array, int index, int count, params T[] newRange)
		{
			var result = new T[array.Length + newRange.Length - count];
			Array.Copy(array, 0, result, 0, index);
			Array.Copy(newRange, 0, result, index, newRange.Length);
			Array.Copy(array, index + count, result, index + newRange.Length, array.Length - index - count);
			return result;
		}
	}
}