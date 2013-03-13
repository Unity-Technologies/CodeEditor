using System.Linq;
using CodeEditor.IO;
using CodeEditor.Testing;
using NUnit.Framework;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services.UnityScript.Tests
{
	[TestFixture]
	public class UnityScriptSourceSymbolProviderTest : MockBasedTest
	{
		[Test]
		public void ReturnsSymbolsForClasses()
		{
			var subject = new UnityScriptSourceSymbolProvider();
			var file = MockFileWithContent("class Foo {}\nclass Bar {}");
			var parsedSymbols = subject.SourceSymbolsFor(file);
			var expectedSymbols = new[]
			{
				new {DisplayText = "Foo", File = file, Line = 1, Column = 7},
				new {DisplayText = "Bar", File = file, Line = 2, Column = 7}
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