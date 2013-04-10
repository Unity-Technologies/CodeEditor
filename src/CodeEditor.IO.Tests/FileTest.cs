using System.IO;
using NUnit.Framework;
using File = CodeEditor.IO.Internal.File;

namespace CodeEditor.IO.Tests
{
	[TestFixture]
	public class FileTest
	{
		[Test]
		public void CanReadAllTextOfFileSharedForRead()
		{
			var fileName = Path.GetTempFileName();
			using (var writer = new StreamWriter(System.IO.File.Open(fileName, FileMode.Open, FileAccess.Write, FileShare.Read)))
			{
				writer.Write("42");
				writer.Flush();
				Assert.AreEqual("42", new File(fileName).ReadAllText());
			}
		}
	}
}
