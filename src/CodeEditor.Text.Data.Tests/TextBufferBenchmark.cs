using System;
using System.Diagnostics;
using NUnit.Framework;

namespace CodeEditor.Text.Data.Tests
{
	[TestFixture]
	public class TextBufferBenchmark : TextBufferBasedTest
	{
		[Test]
		[Ignore("takes a loooooong time")]
		public void Benchmark()
		{
			SetText("");

			const int iterations = 500;
			const int insertionsPerIteration = 30;
			const int deletionsPerIteration = 2;

			var totalTime = 0L;
			var worst = 0L;
			var random = new Randomizer(42);
			for (var i = 0; i < iterations; ++i)
			{
				var watch = Stopwatch.StartNew();

				var insertionPoint = random.Next(Buffer.CurrentSnapshot.Length);
				for (var j = 0; j < insertionsPerIteration; ++j)
					Buffer.Insert(insertionPoint + j, " ");

				var length = Buffer.CurrentSnapshot.Length;
				var readPosition = random.Next(length);
				Buffer.CurrentSnapshot.GetText(readPosition, Math.Min(length - readPosition, insertionsPerIteration));

				for (var j = 0; j < deletionsPerIteration; ++j)
					Buffer.Delete(random.Next(Buffer.CurrentSnapshot.Length), 1);

				watch.Stop();
				var iterationTime = watch.ElapsedMilliseconds;
				if (worst < iterationTime)
					worst = iterationTime;
				totalTime += iterationTime;
			}
			Console.WriteLine("Worst iteration time: {0}ms.", worst);
			Console.WriteLine("Average iteration time: {0}ms.", totalTime / (float)iterations);

			const int totalNumberOfOperations = iterations * (insertionsPerIteration + deletionsPerIteration);
			Console.WriteLine("{0} operations in {1}ms or {2} operations/sec.", totalNumberOfOperations, totalTime, totalNumberOfOperations / (totalTime / 1000.0));

			Assert.AreEqual(iterations * insertionsPerIteration - iterations * deletionsPerIteration, Buffer.CurrentSnapshot.Length);
		}
	}
}