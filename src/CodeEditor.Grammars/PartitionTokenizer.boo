namespace Boo.Ide.Grammars

import Boo.OMeta
import Boo.Adt
import PatternMatching

enum PartitionTokenType:
	None
	Code
	CommentBegin
	CommentContents
	CommentEnd
	CommentLineBegin
	CommentLineContents
	CommentLineEnd
	StringBegin
	StringContents
	StringEnd
	StringSingleBegin
	StringSingleContents
	StringSingleEnd

let ContentTokenTypes = (
	PartitionTokenType.Code,
	PartitionTokenType.CommentContents, PartitionTokenType.CommentLineContents,
	PartitionTokenType.StringContents, PartitionTokenType.StringSingleContents)

def HasContentRole(tokenType as PartitionTokenType):
	return tokenType in ContentTokenTypes

data PartitionToken(Begin as int, End as int, Type as PartitionTokenType) < System.ValueType:
	public Length:
		get: return End - Begin

ometa PartitionTokenizer:

	def Tokenize(last as PartitionTokenType, text as string):
		remaining = OMetaInput.For(text)
		while not remaining.IsEmpty:
			match next(last, remaining):
				case SuccessfulMatch(Input, Value: last):
					yield PartitionToken(remaining.Position, Input.Position, last)
					remaining = Input

	def next(previous as PartitionTokenType, input as OMetaInput):
		match previous:
			case PartitionTokenType.CommentBegin | PartitionTokenType.CommentContents:
				return comment_continue(input)
			case PartitionTokenType.CommentLineBegin | PartitionTokenType.CommentLineContents:
				return comment_line_continue(input)
			case PartitionTokenType.StringBegin | PartitionTokenType.StringContents:
				return string_continue(input)
			case PartitionTokenType.StringSingleBegin | PartitionTokenType.StringSingleContents:
				return string_single_continue(input)
			otherwise:
				return next(input)

	next = code | begin
	code = ++(~begin, _) ^ PartitionTokenType.Code
	begin = comment_begin | comment_line_begin | string_begin | string_single_begin
	comment_begin = '/*' ^ PartitionTokenType.CommentBegin
	comment_continue = ++(~comment_end, _) ^ PartitionTokenType.CommentContents | comment_end
	comment_end = '*/' ^ PartitionTokenType.CommentEnd
	comment_line_begin = ('//' | '#') ^ PartitionTokenType.CommentLineBegin
	comment_line_continue = ++(~'\n', _) ^ PartitionTokenType.CommentLineContents | comment_line_end
	comment_line_end = '\n' ^ PartitionTokenType.CommentLineEnd
	string_begin = '"' ^ PartitionTokenType.StringBegin
	string_continue = ++(~string_end, (string_quote | _)) ^ PartitionTokenType.StringContents | string_end 
	string_quote = '\\"'
	string_end = '"' ^ PartitionTokenType.StringEnd
	string_single_begin = "'" ^ PartitionTokenType.StringSingleBegin
	string_single_continue = ++(~string_single_end, (string_single_quote | _)) ^ PartitionTokenType.StringSingleContents | string_single_end 
	string_single_quote = "\\'"
	string_single_end = "'" ^ PartitionTokenType.StringSingleEnd
