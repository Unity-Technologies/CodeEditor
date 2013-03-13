using CodeEditor.Features.NavigateTo.SourceSymbols.ServiceInterface;
using CodeEditor.Reactive;
using CodeEditor.ReactiveServiceStack;
using ServiceStack.ServiceInterface;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public class SourceSymbolSearchService : AsyncServiceBase<SourceSymbolSearchRequest>
	{
		public ISourceSymbolIndexProvider IndexProvider { get; set; }

		protected override object Run(SourceSymbolSearchRequest request)
		{
			return
				SourceSymbolIndex
					.SearchSymbol(request.Filter)
					.Select(s => new SourceSymbol {DisplayText = s.DisplayText, SourceFile = s.SourceFile.FullName, Line = s.Line-1, Column = s.Column-1})
					.ToJsonStreamWriter();
		}

		ISourceSymbolIndex SourceSymbolIndex
		{
			get { return IndexProvider.Index; }
		}
	}
}