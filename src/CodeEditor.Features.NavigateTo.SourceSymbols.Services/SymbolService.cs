using CodeEditor.Features.NavigateTo.SourceSymbols.ServiceInterface;
using CodeEditor.Reactive;
using CodeEditor.ReactiveServiceStack;
using ServiceStack.ServiceInterface;

namespace CodeEditor.Features.NavigateTo.SourceSymbols.Services
{
	public class SymbolService : AsyncServiceBase<SymbolSearch>
	{
		public IUnityProjectProvider ProjectProvider { get; set; }

		protected override object Run(SymbolSearch request)
		{
			return
				ProjectProvider
					.Project
					.SearchSymbol(request.Filter)
					.Select(s => new Symbol {DisplayText = s.DisplayText, SourceFile = s.SourceFile.FullName, Line = s.Line-1, Column = s.Column-1})
					.ToJsonStreamWriter();
		}
	}
}