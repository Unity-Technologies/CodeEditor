namespace CodeEditor.Collections
{
	public static class Pair
	{
		public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst previous, TSecond next)
		{
			return new Pair<TFirst, TSecond>(previous, next);
		}
	}

	public struct Pair<TFirst, TSecond>
	{
		public readonly TFirst First;
		public readonly TSecond Second;

		public Pair(TFirst first, TSecond second)
		{
			First = first;
			Second = second;
		}
	}
}