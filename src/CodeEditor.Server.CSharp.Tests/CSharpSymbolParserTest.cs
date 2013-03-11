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
			var file = MockFileWithContent("class Foo {}\nclass Bar {}");
			var parsedSymbols = subject.Parse(file);
			var expectedSymbols = new[]
			{
				new {DisplayText = "Foo", File = file, Line = 0, Column = 6},
				new {DisplayText = "Bar", File = file, Line = 1, Column = 6}
			};
			CollectionAssert.AreEquivalent(
				expectedSymbols,
				parsedSymbols.Select(_ => new { _.DisplayText, File = _.SourceFile, _.Line, _.Column }));
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