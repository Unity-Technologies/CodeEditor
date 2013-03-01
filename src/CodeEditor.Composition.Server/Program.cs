using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CodeEditor.Composition.Hosting;
using Funq;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.Providers;
using ServiceStack.WebHost.Endpoints;

namespace CodeEditor.Composition.Server
{
	public class Program
	{
		static void Main()
		{
			Trace.Listeners.Add(new ConsoleTraceListener(true));
			
			using (var pidFileWriter = new StreamWriter(File.Open(PidFile, FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				var urlBase = "http://127.0.0.1:8888/";
				
				pidFileWriter.Write(urlBase);
				pidFileWriter.Flush();

				using (var appHost = new AppHost(DirectoryCatalog.AllAssembliesIn(ServerDirectory)))
				{
					appHost.Init();
					appHost.Start(urlBase);

					Console.WriteLine("Press <ENTER> to quit");
					Console.ReadLine();
				}
			}
		}

		private static string ServerDirectory
		{
			get { return Path.GetDirectoryName(typeof(Program).Module.FullyQualifiedName); }
		}

		protected static string PidFile
		{
			get { return Path.ChangeExtension(typeof(Program).Module.FullyQualifiedName, "pid"); }
		}

		public class AppHost : AppHostHttpListenerBase
		{
			public AppHost(Assembly[] assemblies) : base("CodeEditor.Composition.Server", assemblies)
			{
				CompositionContainer = new CompositionContainer(AssemblyCatalog.For(assemblies));
			}

			protected CompositionContainer CompositionContainer { get; private set; }

			public override void Configure(Container container)
			{
				EndpointHostConfig.Instance.DebugMode = true;
				container.Register<ICompositionContainer>(CompositionContainer);
				container.Register<IRequestLogger>(new InMemoryRollingRequestLogger());
			}
		}
	}
}
