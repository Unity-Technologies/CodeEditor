using System;
using CodeEditor.Composition;

namespace CodeEditor.IO
{
	public interface IShell
	{
		IProcess StartManagedProcess(string executable);
	}

	public interface IProcess : IDisposable
	{
		int Id { get; }
		bool WaitForExit(int timeout);
		void Kill();
	}

	public interface IMonoExecutableProvider
	{
		string MonoExecutable { get; }
	}

	/// <summary>
	/// <see cref="IMonoExecutableProvider"/> implementation that can be used by clients
	/// to override the <see cref="StandardMonoExecutableProvider"/> exported by default
	/// to the container.
	/// </summary>
	public class MonoExecutableProvider : IMonoExecutableProvider
	{
		readonly string _monoExecutable;

		public MonoExecutableProvider(string monoExecutable)
		{
			_monoExecutable = monoExecutable;
		}

		string IMonoExecutableProvider.MonoExecutable
		{
			get { return _monoExecutable; }
		}
	}
	
	/// <summary>
	/// Exported implementation of <see cref="IMonoExecutableProvider"/>.
	/// 
	/// Reads mono executable location from the MONO_EXECUTABLE environment variable.
	/// 
	/// Returns "mono" if MONO_EXECUTABLE is not set.
	/// </summary>
	[Export(typeof(IMonoExecutableProvider))]
	public class StandardMonoExecutableProvider : IMonoExecutableProvider
	{
		public string MonoExecutable
		{
			get { return Environment.GetEnvironmentVariable("MONO_EXECUTABLE") ?? "mono"; }
		}
	}
}