using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CodeEditorWindow : EditorWindow, ICompletionSessionProvider
	{
		public static ITextViewFactory TextViewFactory;

		CodeView _codeView;
		string _fileName;
		ITextView _textView;

		static public void OpenWindowFor(string file)
		{
			var window = GetWindow<CodeEditorWindow>();
			window.title = "Code Editor";
			window.minSize = new Vector2(200, 200);
			window._fileName = file;
		}

		private CodeEditorWindow()
		{
		}

		void InitIfNeeded()
		{
			if (_textView != null)
				return;

			if (string.IsNullOrEmpty(_fileName))
				return;

			_textView = TextViewFactory.ViewForFile(_fileName);
			_codeView = new CodeView(this, _textView);
		}

		void OnGUI()
		{
			InitIfNeeded();
			if (_codeView == null)
				return;

			const float topMargin = 30f;

			if (GUI.Button(new Rect(position.width - 143f, 5f, 65f, 16f), MissingEngineAPI.GUIContent_Temp("Save")))
				_textView.Document.Save();

			var codeViewRect = new Rect(0, topMargin, position.width, position.height - topMargin);
			_codeView.OnGUI(codeViewRect);
		}

		public void StartCompletionSession(TextSpan completionSpan, ICompletionSet completions)
		{
			_codeView.StartCompletionSession(new CompletionSession(completionSpan, completions));
		}
	}
}
