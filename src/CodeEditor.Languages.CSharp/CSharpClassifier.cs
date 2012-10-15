using System.Collections.Generic;
using CodeEditor.Languages.Common;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.CSharp
{
	public class CSharpClassifier : Classifier
	{
		public CSharpClassifier(IStandardClassificationRegistry standardClassificationRegistry, ITextBuffer buffer)
			: base(standardClassificationRegistry, Keywords, buffer)
		{
		}

		private static readonly HashSet<string> Keywords = new HashSet<string>
			{
				"abstract",
				"event",
				"new",
				"struct",
				"as",
				"explicit",
				"null",
				"switch",
				"base",
				"extern",
				"object",
				"this",
				"bool",
				"false",
				"operator",
				"throw",
				"break",
				"finally",
				"out",
				"true",
				"byte",
				"fixed",
				"override",
				"try",
				"case",
				"float",
				"params",
				"typeof",
				"catch",
				"for",
				"private",
				"uint",
				"char",
				"foreach",
				"protected",
				"ulong",
				"checked",
				"goto",
				"public",
				"unchecked",
				"class",
				"if",
				"readonly",
				"unsafe",
				"const",
				"implicit",
				"ref",
				"ushort",
				"continue",
				"in",
				"return",
				"using",
				"decimal",
				"int",
				"sbyte",
				"virtual",
				"default",
				"interface",
				"sealed",
				"volatile",
				"delegate",
				"internal",
				"short",
				"void",
				"do",
				"is",
				"sizeof",
				"while",
				"double",
				"lock",
				"stackalloc",
				"else",
				"long",
				"static",
				"enum",
				"namespace",
				"string",
				"var"
			};
	}
}