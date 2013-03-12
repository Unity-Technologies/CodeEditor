using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.ServiceInterface
{
	public class SymbolSearch : IReturn<IEnumerable<Symbol>>
	{
		public string Filter { get; set; }
	}

	public class Symbol
	{
		public string SourceFile { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public string DisplayText { get; set; }
	}
}
