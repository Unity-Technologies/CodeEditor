using System;
using System.Collections.Generic;

namespace CodeEditor.Composition.Primitives
{
	public interface IExportDefinitionProvider
	{
		IEnumerable<ExportDefinition> GetExports(Type contractType);
	}
}