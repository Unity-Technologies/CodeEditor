using System.Linq;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Server.CSharp.Tests
{
	[TestFixture]
	public class CSharpSymbolSearchTest : MockBasedTest
	{
		[Test]
		[Ignore("wip")]
		public void ReturnsSymbolsForClasses()
		{
			var subject = new CSharpSymbolParser();
			var parsedSymbols = subject.Parse(MockFileWithContent("class Foo {}\nclass Bar {}"));
			CollectionAssert.AreEquivalent(
				new [] { "Foo", "Bar" },
				parsedSymbols.Select(_ => _.DisplayText));
		}

		private IFile MockFileWithContent(string content)
		{
			var file = MockFor<IFile>();
			file
				.Setup(_ => _.ReadAllText())
				.Returns(content);
			return file.Object;
		}
	}
}