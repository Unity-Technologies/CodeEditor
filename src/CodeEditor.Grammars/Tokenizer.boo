namespace Boo.Ide.Grammars

import Boo.OMeta
import Boo.Adt
import PatternMatching

enum TokenType:
	None
	Identifier
	Number
	Punctuation
	WhiteSpace
	
data Token(Begin as int, End as int, Type as TokenType) < System.ValueType

ometa Tokenizer:

	def Tokenize(text as string):
		remaining = OMetaInput.For(text)
		while not remaining.IsEmpty:
			match next(remaining):
				case SuccessfulMatch(Input, Value):
					yield Token(remaining.Position, Input.Position, Value)
					remaining = Input

	next = noise | symbol
	noise = ++(~symbol, _) ^ TokenType.None
	symbol = identifier | number | punctuation | whitespaces
	whitespaces = ++whitespace ^ TokenType.WhiteSpace
	punctuation = (_ >> c and char.IsPunctuation(c)) ^ TokenType.Punctuation
	number = ++digit, (('.', ++digit) | ''), ('f' | 'F' | 'l' | 'L' | '') ^ TokenType.Number
	identifier = prefix, --suffix ^ TokenType.Identifier
	prefix = letter | '_'
	suffix =  prefix | digit
