using System;
using CodeEditor.Composition;

namespace CodeEditor.IO.Internal
{
	[Export(typeof(IMonoExecutableProvider))]
	class StandardMonoExecutableProvider : IMonoExecutableProvider
	{
		public string MonoExecutable
		{
			get { return Environment.GetEnvironmentVariable("MONO_EXECUTABLE") ?? "mono"; }
		}
	}
}