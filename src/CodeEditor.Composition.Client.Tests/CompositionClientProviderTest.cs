using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace CodeEditor.Composition.Client.Tests
{
	[TestFixture]
	public class CompositionClientProviderTest
	{
		private string _serverFolder;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			_serverFolder = CreateTempDirectory();
			CopyServerAssembliesTo(_serverFolder);
		}

		private Assembly AssemblyOf<T>()
		{
			return typeof(T).Assembly;
		}

		private void CopyServerAssembliesTo(string serverFolder)
		{
			var thisModule = GetType().Module.FullyQualifiedName;
			var thisModuleDir = Path.GetDirectoryName(thisModule);
			Func<string, string> buildPathFor = module => Path.Combine(thisModuleDir, "../../../" + module + "/build/Default/");
			CopyFilesTo(serverFolder, buildPathFor("CodeEditor.Composition.Server"), "*.exe", "*.dll");
			CopyFilesTo(serverFolder, thisModuleDir, "*.dll");
		}

		private static void CopyFilesTo(string targetFolder, string sourceFolder, params string[] fileMasks)
		{
			Directory.CreateDirectory(targetFolder);
			foreach (var fileMask in fileMasks)
				foreach (var file in Directory.GetFiles(sourceFolder, fileMask))
					File.Copy(file, Path.Combine(targetFolder, Path.GetFileName(file)), true);
		}

		private ICompositionServerController StartCompositionServerAtFolder(string serverFolder)
		{
			return CompositionServerController.StartCompositionServerAt(serverFolder);
		}

		private string CreateTempDirectory()
		{
			var temp = Path.GetTempFileName();
			File.Delete(temp);
			Directory.CreateDirectory(temp);
			return temp;
		}
	}

	public interface IRemoteService
	{
		int ProcessId { get; }

		void CallMeBackAt(ICallback callback, int value);
	}

	public interface ICallback//<in T>
	{
		void OnNext(int value);
	}

	[Export(typeof(IRemoteService))]
	class RemoteService : MarshalByRefObject, IRemoteService
	{
		public RemoteService()
		{
			ProcessId = Process.GetCurrentProcess().Id;
		}

		public int ProcessId { get; private set; }

		public void CallMeBackAt(ICallback callback, int value)
		{
			callback.OnNext(value);
		}
	}
}
