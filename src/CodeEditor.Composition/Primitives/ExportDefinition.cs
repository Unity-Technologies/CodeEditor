using System;
using System.Collections.Generic;

namespace CodeEditor.Composition.Primitives
{
	public class ExportDefinition : IMetadataProvider
	{
		public ExportDefinition(Type contractType, Type definition)
		{
			ContractType = contractType;
			Implementation = definition;
		}

		public Type ContractType { get; private set; }

		public Type Implementation { get; private set; }

		public IEnumerable<object> Metadata
		{
			get { return Attribute.GetCustomAttributes(Implementation); }
		}
	}
}