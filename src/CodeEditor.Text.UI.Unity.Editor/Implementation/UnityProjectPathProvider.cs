using System.IO;
using CodeEditor.Composition;
using CodeEditor.Languages.Common;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	[Export(typeof(IUnityProjectPathProvider))]
	class UnityProjectPathProvider : IUnityProjectPathProvider
	{
		public string Location
		{
			get
			{
				var location = Path.GetDirectoryName(UnityEngine.Application.dataPath);
				UnityEngine.Debug.Log("IUnityProjectPathProvider.Location -> " + location);
				return location;
			}
		}
	}
}
