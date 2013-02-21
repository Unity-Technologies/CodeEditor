using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
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
			using (var serverProcess = StartCompositionServer())
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
			using (var serverProcess = StartCompositionServer())
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
		}

		private static int CurrentProcessId
		{
			get { return Process.GetCurrentProcess().Id; }
		}

		private static CompositionServerController StartCompositionServer()
		{
			return CompositionServerController.StartCompositionServerAt(Path.GetDirectoryName(CompositionServerExe));
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
