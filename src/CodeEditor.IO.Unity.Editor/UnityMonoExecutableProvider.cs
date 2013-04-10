using System.IO;
using UnityEditor;

namespace CodeEditor.IO.Unity.Editor
{
	/// <summary>
	/// A <see cref="IMonoExecutableProvider"/> that returns the location
	/// of the MonoBleedingEdge executable shipped with Unity.
	/// </summary>
	public class UnityMonoExecutableProvider : MonoExecutableProvider
	{
		public UnityMonoExecutableProvider()
			: base(MonoBleedingEdgeExecutable)
		{
		}

		static string MonoBleedingEdgeExecutable
		{
			get { return Path.Combine(EditorApplication.applicationContentsPath, "MonoBleedingEdge/bin/mono.exe"); }
		}
	}
}