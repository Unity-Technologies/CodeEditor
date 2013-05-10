using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using CodeEditor.Text.UI;
using CodeEditor.Text.UI.Unity.Editor;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Editor.Implementation;

[CustomEditor(typeof(MonoScript))]
class MonoScriptInspector : Editor
{
	CodeView _codeView;
	ITextView _textView;
	SettingsDialog _settingsDialog;
	string _filePath;
	bool _showingSettings;
	int[] _fontSizes = null;

	// For persisting state
	const string _kInspectorCaretRow = "InspectorCaretRow";
	const string _kInspectorCaretColumn = "InspectorCaretColumn";
	const string _kInspectorScroll = "InspectorScroll";
	const string _kInspectorSelectionAnchor = "InspectorSelectionAnchor";

	// Layout
	// ---------------------
	class Styles
	{
		public GUIContent saveText = new GUIContent ("Save");
		public GUIContent optionsIcon = new GUIContent("",EditorGUIUtility.FindTexture("_Popup"));
	}
	static Styles s_Styles;

	/// returns true if the file was opened, false otherwise
	bool OpenFile (string file)
	{
		_textView = null;
		_codeView = null;
		_filePath = file;

		if (string.IsNullOrEmpty(_filePath))
			return false;

		_textView = TextViewFactory.ViewForFile(_filePath);
		_codeView = new CodeView(MissingEditorAPI.currentInspectorWindow, _textView);

		if (_textView != null)
			_settingsDialog = new SettingsDialog(_textView);
		return true;
	}

	public void OnEnable()
	{
		EditorApplication.update += OnInspectorUpdate;
	}

	public void OnInspectorUpdate()
	{
		if (_codeView == null)
			return;

		_codeView.Update();
	}

	static ITextViewFactory TextViewFactory
	{
		get { return UnityEditorCompositionContainer.GetExportedValue<ITextViewFactory>(); }
	}

	bool OpenFile(string file, int row, int column)
	{
		if (!OpenFile(file))
			return false;
		SetPosition(row, column);
		return true;
	}
	
	void InitIfNeeded()
	{
		if (s_Styles == null)
			s_Styles = new Styles ();

		// Reconstruct state after domain reloading
		if (_textView == null && !string.IsNullOrEmpty(_filePath))
		{
			int row = MissingEditorAPI.GetPeristedValueOfType(_kInspectorCaretRow, 0);
			int column = MissingEditorAPI.GetPeristedValueOfType(_kInspectorCaretColumn, 0);
			Vector3 scroll = MissingEditorAPI.GetPeristedValueOfType(_kInspectorScroll, Vector3.zero);
			Vector3 anchor = MissingEditorAPI.GetPeristedValueOfType(_kInspectorSelectionAnchor, Vector3.zero);

			OpenFile(_filePath, row, column);
			_textView.ScrollOffset = scroll;
			_textView.SelectionAnchor = new Position((int)anchor.y, (int)anchor.x);
		}
	}

	void SetPosition(int row, int column)
	{
		_textView.Document.Caret.SetPosition(row, column);
	}

	Color bgColor
	{
		get { return EditorGUIUtility.isProSkin ? new Color (0.2f, 0.2f, 0.2f) : new Color (0.8f, 0.8f, 0.8f);}
	}

	public override void OnInspectorGUI()
	{
		EditorWindow currentWindow = MissingEditorAPI.currentInspectorWindow;
		if (currentWindow == null)
			return;

		Rect position = GUILayoutUtility.GetRect(1f, Screen.width, 1f, Screen.height);
		if (string.IsNullOrEmpty(_filePath) )
			_filePath = AssetDatabase.GetAssetPath (target.GetInstanceID());

		// Draw on top of area of the inspectors that is not needed
		float overlap = 53f;
		position = new Rect(position.x, position.y - overlap, position.width, position.height + overlap);

		DrawRect(position, bgColor);

		InitIfNeeded();

		const float topAreaHeight = 19f;
		Rect topAreaRect = new Rect (position.x, position.y, position.width, topAreaHeight);
		Rect codeViewRect = new Rect(position.x, position.y + topAreaHeight, position.width, position.height - topAreaHeight);

		currentWindow.BeginWindows();
		if (_showingSettings)
			_settingsDialog.OnGUI(codeViewRect);
		TopArea(topAreaRect);
		CodeViewArea(codeViewRect);
		currentWindow.EndWindows();

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

		float buttonHeight = 15f;
		float buttonWidth = 40f;
		Rect r = new Rect(rect.xMax - buttonWidth - 10, rect.y, buttonWidth, buttonHeight);
		if (GUI.Button(r, s_Styles.saveText, EditorStyles.miniButton))
			_textView.Document.Save();

		/*r.Set(rect.xMax - buttonHeight - 10f, r.y, buttonHeight, buttonHeight);
		if (GUI.Button(r, s_Styles.optionsIcon, EditorStyles.label))
		{
			_showingSettings = !_showingSettings;
			Repaint();
		}*/
	}

	void BackupState ()
	{
		if (_textView != null)
		{
			MissingEditorAPI.SetPeristedValueOfType(_kInspectorCaretRow, _textView.Document.Caret.Row);
			MissingEditorAPI.SetPeristedValueOfType(_kInspectorCaretColumn, _textView.Document.Caret.Column);
			MissingEditorAPI.SetPeristedValueOfType(_kInspectorScroll, new Vector3(_textView.ScrollOffset.x, _textView.ScrollOffset.y, 0f));
			MissingEditorAPI.SetPeristedValueOfType(_kInspectorSelectionAnchor, new Vector3(_textView.SelectionAnchor.Column, _textView.SelectionAnchor.Row, 0f));
		}
	}

	void HandleZoomScrolling(Rect rect)
	{
		if (EditorGUI.actionKey && Event.current.type == EventType.scrollWheel && rect.Contains(Event.current.mousePosition))
		{
			if (_fontSizes == null)
				_fontSizes = _textView.FontManager.GetCurrentFontSizes();

			Event.current.Use();

			int sign = Event.current.delta.y > 0 ? -1 : 1;
			int orgSize = _textView.FontManager.CurrentFontSize;
			int index = Array.IndexOf(_fontSizes, orgSize);

			index = Mathf.Clamp(index + sign, 0, _fontSizes.Length-1);
			int newSize = _fontSizes[index];

			if (newSize != orgSize)
			{
				_textView.FontManager.CurrentFontSize = newSize;
				GUI.changed = true;
			}

		}
	}
	
	void DrawRect (Rect rect, Color color)
	{
		Color orgColor = GUI.color;
		GUI.color = color;
		GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
		GUI.color = orgColor;
	}
}
