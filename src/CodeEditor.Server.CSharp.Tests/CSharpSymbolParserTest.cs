using System.Linq;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Server.CSharp.Tests
{
	[TestFixture]
	public class CSharpSymbolParserTest : MockBasedTest
	{
		[Test]
		public void ReturnsSymbolsForClasses()
		{
			var subject = new CSharpSymbolParser();
			var parsedSymbols = subject.Parse(MockFileWithContent("class Foo {}\nclass Bar {}"));
			var expectedSymbols = new[]
			{
				new {DisplayText = "Foo", Line = 1, Column = 7},
				new {DisplayText = "Bar", Line = 2, Column = 7}
			};
			CollectionAssert.AreEquivalent(
				expectedSymbols,
				parsedSymbols.Select(_ => new { _.DisplayText, _.Line, _.Column }));
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