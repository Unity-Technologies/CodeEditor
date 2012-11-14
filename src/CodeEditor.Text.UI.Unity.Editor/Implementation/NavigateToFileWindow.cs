using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	// Open with shortcut
	// Use visual asset style fixed size window
	//	ScrollArea with all files
	//	Search field with focus at startup (all the time), up and down goes to the listarea
	// Enter uses selection, esc closes window
	// Enter feeds selected file to the	 CodeEditor that is opened if not already


	public partial class NavigateToFileWindow : EditorWindow
	{
		const float kSearchBarHeight = 17;
		const float kMargin = 10f;
		const float kLineHeight = 16f;
		string _searchFilter = "";
		ProjectFiles _projectFiles;
		List<FileAndID> _currentNames;
		FileAndID _selectedName;

		class Styles
		{
			public GUIStyle resultsLabel = new GUIStyle ("PR Label");
		}
		static Styles s_Styles;
		private UnityEngine.Vector2 _scrollPosition;
		

		public static void Open ()
		{
			var window = GetWindow<NavigateToFileWindow>();
			window.title = "Navigate To";
		}

		void InitIfNeeded ()
		{
			if (s_Styles == null)
				s_Styles = new Styles();

			if (_projectFiles == null)
			{	
				_projectFiles = new ProjectFiles();
				_projectFiles.Init ();

				_currentNames = _projectFiles.Filter (_searchFilter);
			}
		}

		void OnGUI ()
		{
			InitIfNeeded ();

			Rect searchAreaRect = new Rect (0,position.height-kSearchBarHeight-kMargin, position.width, kSearchBarHeight);
			Rect listAreaRect = new Rect (0, kMargin, position.width, position.height - kSearchBarHeight - 3*kMargin);
			
			HandleKeyboard (); // must be before Search field so we get the key input first
			ListAreaRect (listAreaRect);
			SearchArea (searchAreaRect);
		}

		void OffsetSelection (int offset)
		{
			int index = _currentNames.IndexOf(_selectedName);
			if (index >= 0)
			{
				index += offset;
			}

			index = Mathf.Clamp(index, 0, _currentNames.Count-1);
			Select (_currentNames[index]);
		}

		void HandleKeyboard ()
		{
			Event evt = Event.current;
			switch (evt.type)
			{
				case EventType.KeyDown:
					switch (evt.keyCode)
					{
						case KeyCode.UpArrow:
							OffsetSelection(-1);
							evt.Use();
							break;
						case KeyCode.DownArrow:
							OffsetSelection(1);
							evt.Use();
							break;
						case KeyCode.Return:
							CloseWindow(_selectedName); // close window
							evt.Use();
							break;
						case KeyCode.Escape:
							CloseWindow(null);
							evt.Use();
							break;
					}
				break;
			}
		}

		void CloseWindow (FileAndID fileToOpen)
		{
			if (fileToOpen != null)
			{
				string path = AssetDatabase.GetAssetPath(fileToOpen.InstanceID);
				if (!string.IsNullOrEmpty (path))
				{
					path = System.IO.Path.GetFullPath(path);
					CodeEditorWindow.OpenWindowFor(path);
				}
			}
			Close ();
		}

		void ListAreaRect (Rect rect)
		{
			Event evt = Event.current;

			int firstItem = 0;
			int lastItem = _currentNames.Count-1;

			
			int styleBorder = -1;
			Rect scrollRect = new Rect(rect.x + kMargin, rect.y, rect.width - 2 * kMargin, rect.height);
			GUI.Label (scrollRect, GUIContent.none, GUI.skin.textField);

			scrollRect = new RectOffset (styleBorder,styleBorder,styleBorder,styleBorder).Add (scrollRect);
			Rect contentRect = new Rect(0, 0, 1, kLineHeight * _currentNames.Count);
			_scrollPosition = GUI.BeginScrollView (scrollRect, _scrollPosition, contentRect);
			{
				for (int i=firstItem; i<=lastItem; i++)
				{
					Rect itemRect = new Rect (0, i*kLineHeight, position.width-2*kMargin, kLineHeight);
					
					switch (evt.type)
					{
						case EventType.Repaint:
							bool selected = _currentNames[i] == _selectedName;
							s_Styles.resultsLabel.Draw (itemRect, _currentNames[i].FileName, false, selected, selected, true);
							break;
						case EventType.MouseDown:
							if (itemRect.Contains (evt.mousePosition))
								Select (_currentNames[i]);
							break;
					}
				}

			} GUI.EndScrollView ();

		}

		void Select (FileAndID name)
		{
			_selectedName = name;
			Repaint ();
		}

		void FilterChanged ()
		{
			_currentNames = _projectFiles.Filter (_searchFilter);
			if (_currentNames.Count > 0)
			{
				if (_currentNames.IndexOf (_selectedName) < 0)
					_selectedName = _currentNames[0];
			}
			else
			{
				_selectedName = null;
			}
		}

		// This is our search field
		void SearchArea (Rect rect)
		{
			//GUI.Label (rect, GUIContent.none, s_Styles.toolbarBack);
			Rect searchFieldRect = new Rect (rect.x+ kMargin, rect.y, rect.width - 2*kMargin, rect.height);
			EditorGUI.BeginChangeCheck ();
			{
				GUI.SetNextControlName("SearchFilter");
				_searchFilter = GUI.TextField (searchFieldRect, _searchFilter);
			} 
			if (EditorGUI.EndChangeCheck ())
				FilterChanged ();

			GUI.FocusControl ("SearchFilter");
		}
	}

	public partial class NavigateToFileWindow
	{
		[Serializable]
		class FileAndID 
		{
			public FileAndID(string filename, int instanceID)
			{
				FileName = filename;
				InstanceID = instanceID;
			}
			public string FileName { get; set; }
			public int InstanceID { get; set; }
		}


		class ProjectFiles
		{
			List<FileAndID> _allScripts = new List<FileAndID>();

			public List<FileAndID> AllScripts { get { return _allScripts; } }

			public List<FileAndID> Filter(string filter)
			{
				if (string.IsNullOrEmpty(filter))
					return AllScripts;

				return AllScripts.Where(script => script.FileName.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) >= 0).ToList();
			}

			public void Init()
			{
				MonoScript[] allscripts = MonoImporter.GetAllRuntimeMonoScripts();

				_allScripts.Clear();
				for (int i = 0; i < allscripts.Length; ++i)
				{
					var script = allscripts[i];
					string path = AssetDatabase.GetAssetPath(script.GetInstanceID());
					if (!string.IsNullOrEmpty(path))
					{
						// Scripts can have been removed so ensure index is recalculated based on file path
						path = System.IO.Path.GetFullPath(path);
						_allScripts.Add(new FileAndID(System.IO.Path.GetFileName(path), script.GetInstanceID()));
					}
				}
			}
		}
	} // NavigateToFileWindow
} // namespace 
