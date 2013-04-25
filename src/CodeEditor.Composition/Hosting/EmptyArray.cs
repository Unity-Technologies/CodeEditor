namespace CodeEditor.Composition.Hosting
{
	public static class EmptyArray
	{
		public static T[] Of<T>()
		{
			return ArrayHolder<T>.Array;
		}

		static class ArrayHolder<T>
		{
			public static readonly T[] Array = new T[0];
		}
	}
}