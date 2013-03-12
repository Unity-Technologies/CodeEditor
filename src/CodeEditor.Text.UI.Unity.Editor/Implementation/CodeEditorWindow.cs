using System;
using System.IO;
using System.Collections.Generic;
using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Completion;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CodeEditorWindow : EditorWindow, ICompletionSessionProvider
	{
		public static Func<string, ITextView> TextViewFactory;
		
		[Serializable]
		class BackupData 
		{
			public int caretRow, caretColumn;
			public Vector2 scrollOffset;
			public Vector2 selectionAnchor; // argh: Unity cannot serialize Position since its a struct... so we store it as a Vector2
		}
	
		// Serialized fields (between assembly reloads but NOT between sessions)
		// ---------------------
		string _filePath;
		string _fileNameWithExtension;
		BackupData _backupData;

		// Serialized fields (between assembly reloads AND between sessions)
		[SerializeField]
		int _selectedFontSize = 14;

		// Non serialized fields (reconstructed from serialized state above or recreated when needed)
		// ---------------------
		[NonSerialized]
		CodeView _codeView;
		[NonSerialized]
		ITextView _textView;
		[NonSerialized]
		int[] _fontSizes = null;
		[NonSerialized]
		string[] _fontSizesNames;

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

		// Use of this section requires 4.2 UnityEditor.dll
		/*
		[UnityEditor.Callbacks.OnOpenAsset]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			string assetpath = AssetDatabase.GetAssetPath(instanceID);
			UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetpath, typeof(UnityEngine.Object));
			if(asset is TextAsset || asset is ComputeShader)
			{
				OpenWindowFor(assetpath);
				return true;
			}
			return false;
		}
		*/

		void OpenFile (string file)
		{
			_textView = null;
			_codeView = null;
			_filePath = file;
			_fileNameWithExtension = "";

			if (string.IsNullOrEmpty(_filePath))
				return;

			_textView = TextViewFactory(_filePath);
			_codeView = new CodeView(this, _textView);
			_fileNameWithExtension = Path.GetFileName(_filePath);
		}

		public void OnInspectorUpdate()
		{
			if (_codeView == null)
				return;

			_codeView.Update();
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
				_textView.Appearance.SetFontSize(_selectedFontSize);
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
			HandleZoomScrolling(rect);
			if (_codeView != null)
				_codeView.OnGUI(rect);
		}

		void TopArea(Rect rect)
		{
			if (_textView == null)
				return;

			if(_fontSizes == null)
			{
				_fontSizes = _textView.Appearance.GetSupportedFontSizes();
				var names = new List<string>();
				foreach(int size in _fontSizes)
					names.Add(size.ToString());
				_fontSizesNames = names.ToArray();

			}

			GUILayout.BeginArea(rect);
			{
				GUILayout.BeginVertical ();
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(10f);
					
					GUILayout.Label (_fileNameWithExtension);
				
					GUILayout.FlexibleSpace();

					EditorGUI.BeginChangeCheck();
					_selectedFontSize = EditorGUILayout.IntPopup(_selectedFontSize, _fontSizesNames, _fontSizes, GUILayout.Width(40));
					if(EditorGUI.EndChangeCheck())
						AppearanceChanged();

					if (GUILayout.Button(s_Styles.saveText, EditorStyles.miniButton))
						_textView.Document.Save();

					GUILayout.Space(10f);

				} GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndVertical ();
			} GUILayout.EndArea();
		}

		private void AppearanceChanged()
		{
			_textView.Appearance.SetFontSize(_selectedFontSize);
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

		void HandleZoomScrolling(Rect rect)
		{
			if (EditorGUI.actionKey && Event.current.type == EventType.scrollWheel && rect.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
	
				int sign = Event.current.delta.y > 0 ? -1 : 1;
				int orgSize = _selectedFontSize;
				int index = Array.IndexOf(_fontSizes, _selectedFontSize);

				index = Mathf.Clamp(index + sign, 0, _fontSizes.Length-1);
				_selectedFontSize = _fontSizes[index];

				if (_selectedFontSize != orgSize)
				{
					AppearanceChanged();
					GUI.changed = true;
				}

			}
		}

	}
}
