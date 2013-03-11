using System;

using ServiceStack.Common.Net30;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public class UnityEditorScheduler
	{
		public static UnityEditorScheduler Instance
		{
			get { return Singleton; }
		}

		static readonly UnityEditorScheduler Singleton = new UnityEditorScheduler();

		readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

		UnityEditorScheduler()
		{
			EditorApplication.update += Update;
		}

		public void Schedule(Action action)
		{
			_actions.Enqueue(action);
		}

		private void Update()
		{
			Action action;
			if (_actions.TryDequeue(out action))
				action();
		}
	}
}