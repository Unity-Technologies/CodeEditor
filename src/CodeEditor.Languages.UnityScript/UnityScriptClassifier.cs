using System.Collections.Generic;
using CodeEditor.Languages.Common;
using CodeEditor.Text.Data;
using CodeEditor.Text.Logic;

namespace CodeEditor.Languages.UnityScript
{
	public class UnityScriptClassifier : Classifier
	{
		public static readonly HashSet<string> Keywords = new HashSet<string>
			{
				"byte",
				"sbyte",
				"bool",
				"boolean",
				"int",
				"uint",
				"long",
				"ulong",
				"float",
				"single",
				"double",
				"void",
				"Object",
				"String",
				"as",
				"break",
				"case",
				"catch",
				"class",
				"continue",
				"default",
				"do",
				"else",
				"enum",
				"extends",
				"final",
				"finally",
				"for",
				"function",
				"if",
				"implements",
				"import",
				"in",
				"instanceof",
				"interface",
				"internal",
				"new",
				"override",
				"partial",
				"private",
				"protected",
				"public",
				"return",
				"static",
				"super",
				"switch",
				"this",
				"throw",
				"try",
				"typeof",
				"virtual",
				"var",
				"while",
				"yield",
				"true",
				"false",
				"null"
			};

		public UnityScriptClassifier(IStandardClassificationRegistry standardClassificationRegistry, ITextBuffer buffer)
			: base(standardClassificationRegistry, Keywords, buffer)
		{
		}
	}
}