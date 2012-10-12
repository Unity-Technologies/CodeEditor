using System;
using System.Diagnostics;
using NUnit.Framework;

namespace CodeEditor.Collections.Tests
{
	[TestFixture]
	public class PieceTableBenchmarks
	{
		[Test]
		[Ignore("we don't need to run benchmarks all the time do we?")]
		public void IntensiveEditingSession()
		{
			var table = PieceTable.ForArray(new int[0]);

			const int iterations = 10*1000;
			const int insertionsPerIteration = 30;
			const int deletionsPerIteration = 2;

			var random = new Randomizer(42);
			var totalTime = BenchmarkIterations(iterations, delegate
			{
				var insertionPoint = random.Next(table.Length);
				for (var i = 0; i < insertionsPerIteration; ++i)
					table = table.Insert(insertionPoint + i, i);

				var readPosition = random.Next(table.Length);
				table.ToArray(readPosition, Math.Min(table.Length - readPosition, insertionsPerIteration));

				for (var i = 0; i < deletionsPerIteration; ++i)
					table = table.Delete(random.Next(table.Length), 1);
			});

			const int totalNumberOfOperations = iterations*(insertionsPerIteration + deletionsPerIteration);
			ReportTotalNumberOfOperations(totalTime, totalNumberOfOperations);
			Assert.AreEqual(iterations*insertionsPerIteration - iterations*deletionsPerIteration, table.Length);
		}

		[Test]
		[Ignore("it's a benchmark")]
		public void SequentialAccessAfterManyDeletions()
		{
			const int numberOfDeletions = 100;
			const int numberOfCharacters = 500;
			const int iterations = 10 * 1000;

			var randomizer = new Randomizer(42);

			var table = PieceTable.ForString(new string(' ', 5 * 1024));
			for (var i = 0; i < numberOfDeletions; ++i)
				table = table.Delete(randomizer.Next(table.Length), 1);

			Console.WriteLine("** CopyTo iteration **");
			var buffer = new char[1];
			var totalWithout = BenchmarkIterations(iterations, delegate
			{
				var position = randomizer.Next(table.Length - numberOfCharacters * 2) + numberOfCharacters;
				for (var i = 0; i < numberOfCharacters; ++i)
					table.CopyTo(buffer, 0, position + i, 1);
				for (var i = 0; i < numberOfCharacters; ++i)
					table.CopyTo(buffer, 0, position - i, 1);
			});
			const int totalNumberOfOperations = iterations*numberOfCharacters;
			ReportTotalNumberOfOperations(totalWithout, totalNumberOfOperations);

			Console.WriteLine("** Point iteration **");
			var totalWith = BenchmarkIterations(iterations, delegate
			{
				var position = randomizer.Next(table.Length - numberOfCharacters * 2) + numberOfCharacters;
				var pivot = table[position];
				var current = pivot;
				for (var i = 0; i < numberOfCharacters; ++i)
				{
					current.GetValue();
					current = current.Next;
				}
				current = pivot;
				for (var i = 0; i < numberOfCharacters; ++i)
				{
					current.GetValue();
					current = current.Previous;
				}
			});
			ReportTotalNumberOfOperations(totalWith, totalNumberOfOperations);
		}

		private static long BenchmarkIterations(int iterations, Action iteration)
		{
			var totalTime = 0L;
			var worst = 0L;
			for (var i = 0; i < iterations; ++i)
			{
				var watch = Stopwatch.StartNew();
				iteration();
				watch.Stop();
				var iterationTime = watch.ElapsedMilliseconds;
				if (iterationTime > worst) worst = iterationTime;
				totalTime += iterationTime;
			}
			Console.WriteLine("Worst iteration time: {0}ms.", worst);
			Console.WriteLine("Average iteration time: {0}ms.", totalTime/(float) iterations);
			return totalTime;
		}

		private static void ReportTotalNumberOfOperations(long totalTime, int totalNumberOfOperations)
		{
			Console.WriteLine("{0} operations in {1}ms or {2} operations/sec.",
			                  totalNumberOfOperations,
			                  totalTime,
			                  totalNumberOfOperations/(totalTime/1000.0));
		}
	}
}
