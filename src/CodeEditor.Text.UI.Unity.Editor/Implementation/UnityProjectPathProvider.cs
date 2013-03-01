using System.IO;
using CodeEditor.Composition;
using CodeEditor.Languages.Common;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IUnityProjectPathProvider))]
	class UnityProjectPathProvider : IUnityProjectPathProvider
	{
		public string Location
		{
			get
			{
				return Path.GetDirectoryName(UnityEngine.Application.dataPath);
			}
		}
	}
}
