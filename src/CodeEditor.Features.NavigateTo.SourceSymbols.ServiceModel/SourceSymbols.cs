using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.ServiceModel
{
	public class SourceSymbolSearchRequest : IReturn<IEnumerable<SourceSymbol>>
	{
		public string Filter { get; set; }
	}

	public class SourceSymbol
	{
		public string SourceFile { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public string DisplayText { get; set; }
	}
}
