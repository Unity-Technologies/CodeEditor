using System.Collections.Generic;
using CodeEditor.Languages.Common;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.Boo
{
	public class BooClassifier : Classifier
	{
		public BooClassifier(IStandardClassificationRegistry standardClassificationRegistry, ITextBuffer buffer)
			: base(standardClassificationRegistry, Keywords, buffer)
		{
		}

		private static readonly HashSet<string> Keywords = new HashSet<string>
			{
				"byte",
				"sbyte",
				"bool",
				"int",
				"uint",
				"long",
				"ulong",
				"single",
				"double",
				"void",
				"object",
				"duck",
				"string",
				"import",
				"namespace",
				"as",
				"from",
				"of",
				"abstract",
				"virtual",
				"final",
				"override",
				"partial",
				"private",
				"protected",
				"public",
				"internal",
				"new",
				"static",
				"and",
				"break",
				"cast",
				"callable",
				"continue",
				"constructor",
				"class",
				"def",
				"do",
				"else",
				"enum",
				"ensure",
				"event",
				"except",
				"for",
				"goto",
				"if",
				"is",
				"isa",
				"in",
				"interface",
				"not",
				"or",
				"pass",
				"raise",
				"ref",
				"return",
				"struct",
				"try",
				"typeof",
				"unless",
				"while",
				"yield",
				"get",
				"set",
				"true",
				"false",
				"null",
				"super",
				"self",
				"property",
				"getter",
				"required",
				"match",
				"case",
				"otherwise",
				"assert",
				"print",
				"macro",
				"using",
				"yieldAll",
				"lock",
				"array",
				"matrix",
				"len"
			};
	}
}