using System;
using System.Diagnostics;
using System.IO;
using CodeEditor.Composition.Hosting;

namespace CodeEditor.Composition.Server
{
	public class CompositionContainerServer : MarshalByRefObject, IServiceProvider
	{
		readonly CompositionContainer _container;

		public CompositionContainerServer()
		{
			_container = new CompositionContainer(new DirectoryCatalog(Path.GetDirectoryName(GetType().Module.FullyQualifiedName)));
		}

		public object GetService(Type serviceType)
		{
			Trace.WriteLine(string.Format("GetService({0})", serviceType));
			return _container.GetExportedValue(serviceType);
		}
	}
}