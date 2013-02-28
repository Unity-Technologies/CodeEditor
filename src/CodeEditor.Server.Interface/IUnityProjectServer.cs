using System;
using CodeEditor.Reactive;

namespace CodeEditor.Server.Interface
{
	public interface IUnityProjectServer
	{
		IUnityProject ProjectForFolder(string projectFolder);
	}

	public interface IUnityProject
	{
		IObservableX<ISymbol> SearchSymbol(string pattern);
	}

	public interface ISymbol
	{
		int Line { get; }
		int Column { get; }
		string DisplayText { get; }
	}

	[Serializable]
	public class Symbol : ISymbol
	{
		public int Line
		{
			get { return 1; }
		}

		public int Column
		{
			get { return 7; }
		}

		public string DisplayText
		{
			get { return "Foo"; }
		}
	}
}
