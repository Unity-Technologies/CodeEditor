using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using CodeEditor.Composition.Hosting;
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

		[Test]
		public void GetService_Activates_Service_On_Server_Process()
		{
			using (var serverProcess = StartCompositionServerAtFolder(_serverFolder))
			{
				var subject = CreateCompositionClientProvider();
				var client = subject.CompositionClientFor("tcp://localhost:8888/IServiceProvider");
				var service = client.GetService<IRemoteService>();
				Assert.AreEqual(serverProcess.Id, service.ProcessId);
			}
		}

		[Test]
		public void CallbacksAreSupported()
		{
			using (var serverProcess = StartCompositionServerAtFolder(_serverFolder))
			{
				var subject = CreateCompositionClientProvider();
				var client = subject.CompositionClientFor("tcp://localhost:8888/IServiceProvider");

				var callbackWaitHandle = new ManualResetEvent(false);
				var callback = new RemoteCallback(_ => callbackWaitHandle.Set());
				var service = client.GetService<IRemoteService>();
				service.CallMeBackAt(callback, CurrentProcessId);

				Assert.IsTrue(callbackWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
				Assert.AreEqual(CurrentProcessId, callback.LastValue);
			}
		}

		public class RemoteCallback : MarshalByRefObject, ICallback
		{
			private readonly Action<int> _action;
			private int _lastValue;

			public RemoteCallback(Action<int> action)
			{
				_action = action;
			}

			public int LastValue
			{
				get { return _lastValue; }
			}

			public void OnNext(int value)
			{
				_lastValue = value;
				_action(value);
			}
		}
		/*
		[Test]
		public void RemotableObservable()
		{
			using (var serverProcess = StartCompositionServer())
			{
				var subject = CreateCompositionClientProvider();
				var client = subject.CompositionClientFor("tcp://localhost:8888/IServiceProvider");
				var service = client.GetService<IPingPonger>();
				var pongs = new ConcurrentBag<Pong>();
				using (service.Pong.Subscribe(pongs.Add))
				{
					Assert.AreEqual(0, pongs.Count);

					service.Ping(CurrentProcessId);

					Pong pong;
					Assert.IsTrue(pongs.TryTake(out pong));
					Assert.AreEqual(CurrentProcessId, pong.PingerId);
					Assert.AreEqual(serverProcess.Id, pong.PongerId);
				}
				service.Ping(CurrentProcessId);
				Assert.AreEqual(0, pongs.Count);
			}
		}*/

		private static int CurrentProcessId
		{
			get { return Process.GetCurrentProcess().Id; }
		}

		private ICompositionClientProvider CreateCompositionClientProvider()
		{
			return new CompositionContainer(AssemblyOf<ICompositionClientProvider>())
				.GetExportedValue<ICompositionClientProvider>();
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
