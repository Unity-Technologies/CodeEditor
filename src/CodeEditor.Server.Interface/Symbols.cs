using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace CodeEditor.Server.Interface
{
	[Route("/symbols")]
	public class SymbolSearch : IReturn<IEnumerable<Symbol>>
	{
		public string Filter { get; set; }
	}

	public class Symbol
	{
		public string DisplayText { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
	}
}
