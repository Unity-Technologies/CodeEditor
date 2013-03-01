using System;

using ServiceStack.Common.Net30;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class UnityEditorScheduler : IDisposable
	{
		readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

		public UnityEditorScheduler()
		{
			EditorApplication.update += Update;
		}

		public void Schedule(Action action)
		{
			_actions.Enqueue(action);
		}

		public void Dispose()
		{
			EditorApplication.update -= Update;
		}

		private void Update()
		{
			Action action;
			if (_actions.TryDequeue(out action))
				action();
		}
	}
}