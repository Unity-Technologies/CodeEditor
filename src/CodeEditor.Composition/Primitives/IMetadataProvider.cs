using System.Collections.Generic;

namespace CodeEditor.Composition.Primitives
{
	public interface IMetadataProvider
	{
		IEnumerable<object> Metadata { get; }
	}
}