using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CodeEditorWindow : EditorWindow, ICompletionSessionProvider
	{
		public static ITextViewFactory TextViewFactory;
		
		[System.Serializable]
		class BackupData 
		{
			// TextView state that should survive domain reloads
			public int caretRow, caretColumn;
			public Vector2 scrollOffset;
			public Vector2 selectionAnchor; // argh: Unity cannot serialize Position since its a struct... so we store it as a Vector2
		}
	
		// Serialized fields
		// ---------------------
		string _filePath;
		string _fileNameWithExtension;
		BackupData _backupData;


		// Non serialized fields (reconstructed from serialized state above or recreated when needed)
		// ---------------------
		[System.NonSerialized]
		CodeView _codeView;

		[System.NonSerialized]
		ITextView _textView;
		


		// Layout
		// ---------------------
		class Styles
		{
			public GUIContent saveText = new GUIContent ("Save");
		}
		static Styles s_Styles;

		static public CodeEditorWindow OpenOrFocusExistingWindow()
		{
			var window = GetWindow<CodeEditorWindow>();
			window.title = "Code Editor";
			window.minSize = new Vector2(200, 200);
			return window;
		}

		static public void OpenWindowFor(string file)
		{
			var window = OpenOrFocusExistingWindow ();
			window.OpenFile (file);
		}

		private CodeEditorWindow()
		{
		}

		void OpenFile (string file)
		{
			_textView = null;
			_codeView = null;
			_filePath = file;
			_fileNameWithExtension = "";

			if (string.IsNullOrEmpty(_filePath))
				return;

			_textView = TextViewFactory.ViewForFile(_filePath);
			_codeView = new CodeView(this, _textView);
			_fileNameWithExtension = System.IO.Path.GetFileName(_filePath);
		}

		void InitIfNeeded()
		{
			if (s_Styles == null)
				s_Styles = new Styles ();

			if (_backupData == null)
				_backupData = new BackupData(); 

			// Reconstruct state after domain reloading
			if (_textView == null && !string.IsNullOrEmpty(_filePath))
			{
				OpenFile (_filePath);
				_textView.Document.Caret.SetPosition(_backupData.caretRow, _backupData.caretColumn);
				_textView.ScrollOffset = _backupData.scrollOffset;
				_textView.SelectionAnchor = new Position((int)_backupData.selectionAnchor.y, (int)_backupData.selectionAnchor.x);
			}
		}

		void OnGUI()
		{
			InitIfNeeded();

			const float topAreaHeight = 30f;
			Rect topAreaRect = new Rect (0,0, position.width, topAreaHeight);
			Rect codeViewRect = new Rect(0, topAreaHeight, position.width, position.height - topAreaHeight);
			
			TopArea(topAreaRect);
			CodeViewArea(codeViewRect);
			
			BackupState ();
		}

		void CodeViewArea (Rect rect)
		{
			if (_codeView != null)
				_codeView.OnGUI(rect);
		}

		void TopArea(Rect rect)
		{
			GUILayout.BeginArea(rect);
			{
				GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(10f);
					
					GUILayout.Label (_fileNameWithExtension);
				
					GUILayout.FlexibleSpace();

					if (GUILayout.Button(s_Styles.saveText))
						_textView.Document.Save();

					GUILayout.Space(10f);

				} GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical ();
			} GUILayout.EndArea();
		}

		void BackupState ()
		{
			if (_textView != null)
			{
				_backupData.caretRow = _textView.Document.Caret.Row;
				_backupData.caretColumn = _textView.Document.Caret.Column;
				_backupData.scrollOffset = _textView.ScrollOffset;
				_backupData.selectionAnchor = new Vector2(_textView.SelectionAnchor.Column, _textView.SelectionAnchor.Row);
			}
		}

		public void StartCompletionSession(TextSpan completionSpan, ICompletionSet completions)
		{
			_codeView.StartCompletionSession(new CompletionSession(completionSpan, completions));
		}
	}
}
