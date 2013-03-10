using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using CodeEditor.Composition.Client.Tests.Fixtures;
using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.Composition.Client.Tests
{
	[TestFixture]
	public class CompositionClientProviderTest
	{
		[Test]
		public void GetService_Activates_Service_On_Server_Process()
		{
			var serverProcess = StartCompositionServer();
			try
			{
				var subject = CreateCompositionClientProvider();
				var client = subject.CompositionClientFor("tcp://localhost:8888/IServiceProvider");
				var service = client.GetService<IRemoteService>();
				Assert.AreEqual(serverProcess.Id, service.ProcessId);
			}
			finally
			{
				StopCompositionServer(serverProcess);
			}
		}

		[Test]
		public void CallbacksAreSupported()
		{
			var serverProcess = StartCompositionServer();
			try
			{
				var subject = CreateCompositionClientProvider();
				var client = subject.CompositionClientFor("tcp://localhost:8888/IServiceProvider");

				var callbackWaitHandle = new ManualResetEvent(false);
				var callback = new RemoteCallback<int>(_ => callbackWaitHandle.Set());
				var service = client.GetService<IRemoteService>();
				service.CallMeBackAt(callback, CurrentProcessId);

				Assert.IsTrue(callbackWaitHandle.WaitOne(TimeSpan.FromSeconds(1)));
				Assert.AreEqual(CurrentProcessId, callback.LastValue);
			}
			finally
			{
				StopCompositionServer(serverProcess);
			}
		}

		public class RemoteCallback<T> : MarshalByRefObject, ICallback<T>
		{
			private readonly Action<T> _action;
			private T _lastValue;

			public RemoteCallback(Action<T> action)
			{
				_action = action;
			}

			public T LastValue
			{
				get { return _lastValue; }
			}

			public void OnNext(T value)
			{
				_lastValue = value;
				_action(value);
			}
		}

		[Test]
		public void RemotableObservable()
		{
			var serverProcess = StartCompositionServer();
			try
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
			finally
			{
				StopCompositionServer(serverProcess);
			}
		}

		private static int CurrentProcessId
		{
			get { return Process.GetCurrentProcess().Id; }
		}

		private static Process StartCompositionServer()
		{
			return Process.Start(new ProcessStartInfo(CompositionServerExe)
			{
				RedirectStandardInput = true,
				UseShellExecute = false
			});
		}

		private static void StopCompositionServer(Process serverProcess)
		{
			serverProcess.StandardInput.WriteLine("quit");
			Assert.IsTrue(serverProcess.WaitForExit(2000));
		}

		private ICompositionClientProvider CreateCompositionClientProvider()
		{
			return new CompositionContainer(AssemblyOf<ICompositionClientProvider>())
				.GetExportedValue<ICompositionClientProvider>();
		}

		private static string CompositionServerExe
		{
			get { return typeof(Server.Program).Module.FullyQualifiedName; }
		}

		private Assembly AssemblyOf<T>()
		{
			return typeof(T).Assembly;
		}
	}

	public interface IRemoteService
	{
		int ProcessId { get; }

		void CallMeBackAt<T>(ICallback<T> callback, T value);
	}

	public interface ICallback<in T>
	{
		void OnNext(T value);
	}

	[Export(typeof(IRemoteService))]
	class RemoteService : MarshalByRefObject, IRemoteService
	{
		public RemoteService()
		{
			ProcessId = Process.GetCurrentProcess().Id;
		}

		public int ProcessId { get; private set; }

		public void CallMeBackAt<T>(ICallback<T> callback, T value)
		{
			callback.OnNext(value);
		}
	}
}
