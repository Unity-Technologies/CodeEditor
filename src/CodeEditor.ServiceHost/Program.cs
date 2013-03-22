using System;
using System.Collections.Generic;
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

		static string ServerDirectory
		{
			get { return Path.GetDirectoryName(FullyQualifiedName); }
		}

		protected static string UriFilePath
		{
			get { return Path.ChangeExtension(FullyQualifiedName, "uri"); }
		}

		static string FullyQualifiedName
		{
			get { return typeof(Program).Module.FullyQualifiedName; }
		}

		public class AppHost : AppHostHttpListenerBase
		{
			public AppHost(Assembly[] assemblies)
				: base(typeof(AppHost).Namespace, AssembliesWithServicesFrom(assemblies))
			{
				CompositionContainer = new CompositionContainer(AssemblyCatalog.For(assemblies));
			}

			CompositionContainer CompositionContainer { get; set; }

			public override void Configure(Container container)
			{
				EndpointHostConfig.Instance.DebugMode = true;
				container.Adapter = new CompositionContainerAdapter(CompositionContainer);
				container.Register<IRequestLogger>(new InMemoryRollingRequestLogger());
			}

			public class CompositionContainerAdapter : IContainerAdapter
			{
				readonly CompositionContainer _compositionContainer;

				public CompositionContainerAdapter(CompositionContainer compositionContainer)
				{
					_compositionContainer = compositionContainer;
				}

				T IContainerAdapter.TryResolve<T>()
				{
					var singleOrDefaultExport = _compositionContainer.GetExports(typeof(T)).SingleOrDefault();
					if (singleOrDefaultExport == null)
						return default(T);
					return (T)singleOrDefaultExport.Value;
				}

				T IContainerAdapter.Resolve<T>()
				{
					return _compositionContainer.GetExportedValue<T>();
				}
			}

			static Assembly[] AssembliesWithServicesFrom(IEnumerable<Assembly> assemblies)
			{
				var serviceInterfaceAssembly = typeof(IService).Assembly;
				var serviceAssembly = typeof(ServiceStack.ServiceInterface.Service).Assembly;
				return assemblies
					.Where(_ => _.References(serviceInterfaceAssembly) || _.References(serviceAssembly))
					.ToArray();
			}
		}
	}
}