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
			var exportedValue = _container.GetExportedValue(serviceType);
			if (!(exportedValue is MarshalByRefObject))
				throw new InvalidOperationException(string.Format("Type '{0}' must inherit from System.MarshalByRefObject to be served.", exportedValue.GetType()));
			return exportedValue;
		}
	}
}