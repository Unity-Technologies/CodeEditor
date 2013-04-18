using CodeEditor.Testing;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using NUnit.Framework;

/*
 TODO test:
		int ConvertToGraphicalCaretColumn(int logicalCaretColumn, ITextViewLine line);
		int ConvertToLogicalCaretColumn(int graphicalCaretColumn, ITextViewLine line);
*/

namespace CodeEditor.Text.UI.Unity.Engine.Tests
{
	[TestFixture]
	class TextViewWhitespaceTest : MockBasedTest
	{
		public ITextViewWhitespace GetTextViewWhitespace()
		{
			return new TextViewWhitespace(new BoolSetting("visible", true, null), new IntSetting("numSpaces", 4, null));
		}

		void TestBaseTextFormatting(int numberOfSpacesPerTab, string input, string expectedResult)
		{
			ITextViewWhitespace whitespace = GetTextViewWhitespace();
			whitespace.NumberOfSpacesPerTab = numberOfSpacesPerTab;
			string result = whitespace.FormatBaseText(input);
			Assert.AreEqual(result, expectedResult);
		}

		void TestRichTextFormating (int numberOfSpacesPerTab, string baseText, string richText, string expectedResult)
		{
			ITextViewWhitespace whitespace = GetTextViewWhitespace();
			whitespace.NumberOfSpacesPerTab = numberOfSpacesPerTab;
			string result = whitespace.FormatRichText(richText, whitespace.GetTabSizes(baseText));
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public void TestTabularStopsIsWorking()
		{
			TestBaseTextFormatting(4, "\tOO\tOO",       "    OO  OO");
			TestBaseTextFormatting(4, "OO\tO\tOO",      "OO  O   OO");
			TestBaseTextFormatting(4, "O\tOOO\tOO\t",   "O   OOO OO  ");
			TestBaseTextFormatting(4, "O\tOO\tOO\t\t",  "O   OO  OO      ");

			TestBaseTextFormatting(2, "\tI\tII",     "  I II");
			TestBaseTextFormatting(2, "I\tII\tII",   "I II  II");
			TestBaseTextFormatting(2, "I\tII\tII\t", "I II  II  ");
			TestBaseTextFormatting(2, "\tI\tII\t\t", "  I II    ");
		}

		[Test]
		public void TestRichTextFormating()
		{
			int numberOfSpacesPerTab = 4;
			string baseText = "\tFooText\t";
			string richText = "\t<color=#787855>FooText</color>\t";
			string richTextExpected = "    <color=#787855>FooText</color> ";
			TestRichTextFormating(numberOfSpacesPerTab, baseText, richText, richTextExpected);
		}

		[Test]
		public void TestShowVisibleWhitespace()
		{
			ITextViewWhitespace whitespace = GetTextViewWhitespace();
			whitespace.NumberOfSpacesPerTab = 4;
			whitespace.Visible = true;
			string result = whitespace.FormatBaseText("\t O \t OO");
			Assert.AreEqual(result[0], whitespace.VisibleTabChar);
			Assert.AreEqual(result[4], whitespace.VisibleSpaceChar);
			Assert.AreEqual(result[7], whitespace.VisibleTabChar);
		}
	}
}
