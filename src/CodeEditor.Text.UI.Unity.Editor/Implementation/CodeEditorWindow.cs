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
		}
	
		// Serialized fields
		// ---------------------
		string _fileName;
		BackupData _backupData;


		// Non serialized fields (reconstructed from serialized state above or recreated when needed)
		// ---------------------
		[System.NonSerialized]
		CodeView _codeView;

		[System.NonSerialized]
		ITextView _textView;
	
		[System.NonSerialized]
		int _selectedScriptIndex;
		
		[System.NonSerialized]
		MonoScript[] _allScripts;
		
		[System.NonSerialized]
		string[] _allScriptNames;
		
		// Layout
		// ---------------------
		class Styles
		{
			public GUIContent saveText = new GUIContent ("Save");
		}
		static Styles s_Styles;



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

		string GetFileFromList (int index)
		{
			if (index < 0 || index >= _allScripts.Length)
				return "";

			var assetPath = AssetDatabase.GetAssetPath(_allScripts[index].GetInstanceID());
			return System.IO.Path.GetFullPath(assetPath);
		}

		void OpenFile (string file)
		{
			_textView = null;
			_codeView = null;
			_fileName = file;

			if (string.IsNullOrEmpty(_fileName))
				return;

			_textView = TextViewFactory.ViewForFile(_fileName);
			_codeView = new CodeView(this, _textView);
		}

		void InitIfNeeded()
		{
			if (_allScripts != null)
				return;
	
			if (s_Styles == null)
				s_Styles = new Styles ();

			if (_backupData == null)
				_backupData = new BackupData(); 

			MonoScript[] allscripts = MonoImporter.GetAllRuntimeMonoScripts ();
			List<string> scriptNames = new List<string>();
			List<MonoScript> scripts= new List<MonoScript>();
			_selectedScriptIndex = -1;

			for (int i=0; i<allscripts.Length; ++i)
			{
				var script = allscripts[i];
				string path = AssetDatabase.GetAssetPath (script.GetInstanceID ());
				if (!string.IsNullOrEmpty (path))
				{
					// Scripts can have been removed so ensure index is recalculated based on file path
					path = System.IO.Path.GetFullPath(path);
					if (path == _fileName)
						_selectedScriptIndex = i;
					scriptNames.Add (System.IO.Path.GetFileName (path));
					scripts.Add (script);
				}
			}
			_allScriptNames = scriptNames.ToArray ();
			_allScripts = scripts.ToArray ();

			// Reconstruct state
			if (_selectedScriptIndex >= 0)
			{
				OpenFile (GetFileFromList (_selectedScriptIndex));
				_textView.Document.Caret.SetPosition (_backupData.caretRow, _backupData.caretColumn);
				_textView.ScrollOffset = _backupData.scrollOffset;
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
					
					EditorGUI.BeginChangeCheck ();
					_selectedScriptIndex = EditorGUILayout.Popup(_selectedScriptIndex, _allScriptNames);
					if (EditorGUI.EndChangeCheck())
						OpenFile (GetFileFromList(_selectedScriptIndex));
					
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
			_backupData.caretRow = _textView.Document.Caret.Row;
			_backupData.caretColumn = _textView.Document.Caret.Column;
			_backupData.scrollOffset = _textView.ScrollOffset;
		}

		public void StartCompletionSession(TextSpan completionSpan, ICompletionSet completions)
		{
			_codeView.StartCompletionSession(new CompletionSession(completionSpan, completions));
		}
	}
}
