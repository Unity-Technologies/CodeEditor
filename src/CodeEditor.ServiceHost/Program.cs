using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeEditor.Composition.Hosting;
using Funq;
using ServiceStack.Configuration;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.Providers;
using ServiceStack.WebHost.Endpoints;

namespace CodeEditor.ServiceHost
{
	public class Program
	{
		static void Main()
		{
			using (var uriFileWriter = new StreamWriter(File.Open(UriFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				const string baseUri = "http://localhost:8888/";
				
				uriFileWriter.WriteLine(baseUri);
				uriFileWriter.Flush();

				using (var appHost = new AppHost(DirectoryCatalog.AllAssembliesIn(ServerDirectory)))
				{
					appHost.Init();
					appHost.Start(baseUri);

					Console.WriteLine("Press <ENTER> to quit");
					Console.ReadLine();
				}
			}
		}

		private static string ServerDirectory
		{
			get { return Path.GetDirectoryName(FullyQualifiedName); }
		}

		protected static string UriFilePath
		{
			get { return Path.ChangeExtension(FullyQualifiedName, "uri"); }
		}

		private static string FullyQualifiedName
		{
			get { return typeof(Program).Module.FullyQualifiedName; }
		}

		public class AppHost : AppHostHttpListenerBase
		{
			public AppHost(Assembly[] assemblies) : base(typeof(AppHost).Namespace, assemblies.Where(_ => _.References(typeof(IService).Assembly)).ToArray())
			{
				CompositionContainer = new CompositionContainer(AssemblyCatalog.For(assemblies));
			}

			protected CompositionContainer CompositionContainer { get; private set; }

			public override void Configure(Container container)
			{
				EndpointHostConfig.Instance.DebugMode = true;
				container.Adapter = new CompositionContainerAdapter(CompositionContainer);
				container.Register<IRequestLogger>(new InMemoryRollingRequestLogger());
			}

			public class CompositionContainerAdapter : IContainerAdapter
			{
				private readonly CompositionContainer _compositionContainer;

				public CompositionContainerAdapter(CompositionContainer compositionContainer)
				{
					_compositionContainer = compositionContainer;
				}

				T IContainerAdapter.TryResolve<T>()
				{
					var singleOrDefaultExport = _compositionContainer.GetExports(typeof(T)).SingleOrDefault();
					if (singleOrDefaultExport == null)
						return default(T);
					return (T) singleOrDefaultExport.Value;
				}

				T IContainerAdapter.Resolve<T>()
				{
					return _compositionContainer.GetExportedValue<T>();
				}
			}
		}
	}
}
