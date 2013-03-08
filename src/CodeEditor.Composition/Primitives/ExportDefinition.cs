using System;
using System.Collections.Generic;

namespace CodeEditor.Composition.Primitives
{
	public class ExportDefinition : IMetadataProvider
	{
		public static IEnumerable<object> DefaultMetadataForType(Type type)
		{
			return Attribute.GetCustomAttributes(type);
		}

		readonly Lazy<IEnumerable<object>> _metadata;

		public ExportDefinition(Type contractType, Type implementation)
			: this(contractType, implementation, () => DefaultMetadataForType(implementation))
		{
		}

		public ExportDefinition(Type contractType, Type implementation, Func<IEnumerable<object>> metadataFactory)
		{
			ContractType = contractType;
			Implementation = implementation;
			_metadata = new Lazy<IEnumerable<object>>(metadataFactory);
		}

		public Type ContractType { get; private set; }

		public Type Implementation { get; private set; }

		public IEnumerable<object> Metadata
		{
			get { return _metadata.Value; }
		}
	}
}