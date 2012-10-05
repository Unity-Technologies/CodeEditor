using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeEditor.Composition.Primitives;

namespace CodeEditor.Composition.Hosting
{
	public class DirectoryCatalog : IExportDefinitionProvider
	{
		private readonly IExportDefinitionProvider _catalog;

		public DirectoryCatalog(string directory)
		{
			_catalog = AssemblyCatalog.For(AllAssembliesIn(directory));
		}

		public IEnumerable<ExportDefinition> GetExports(Type contractType)
		{
			return _catalog.GetExports(contractType);
		}

		private static Assembly[] AllAssembliesIn(string directory)
		{
			return Directory.GetFiles(directory, "*.dll").Select(s => Assembly.LoadFrom(s)).ToArray();
		}
	}
}
