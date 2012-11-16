using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public partial class NavigateToFileWindow : EditorWindow
	{
		const float kSearchBarHeight = 17;
		const float kMargin = 10f;
		const float kLineHeight = 16f;
		
		IFilePathProvider _filePathProvider;
		List<FilePathProviderItem> _currentItems;
		FilePathProviderItem _selectedItem;
		string _searchFilter = "";
		private Vector2 _scrollPosition;
		string _filePathProviderQualifiedName; // stored as string so we can serialize and reconstruct a filePathProvider after domain reloads

		[NonSerialized]
		bool _readyToInit = false;

		class Styles
		{
			public GUIStyle resultsLabel = new GUIStyle ("PR Label");
			public Styles()
			{
				resultsLabel.padding.left = 5;
			}
		}
		static Styles s_Styles;


		public static void Open (IFilePathProvider filePathProvider)
		{
			var window = GetWindow<NavigateToFileWindow>();
			window.title = "Navigate To";
			window._filePathProviderQualifiedName = filePathProvider.GetType().AssemblyQualifiedName;
		}

		static IFilePathProvider CreateProvider(string assemblyQualifiedName)
		{
			IFilePathProvider provider = null;
			Type t = Type.GetType(assemblyQualifiedName);
			if (t != null)
				provider = Activator.CreateInstance(t) as IFilePathProvider;

			if (provider == null)
				Debug.LogError ("Could not create a FilePathProvider from " + assemblyQualifiedName);
			return provider;
		}

		void InitIfNeeded ()
		{
			if (s_Styles == null)
				s_Styles = new Styles();

			if (_filePathProvider == null)
				_filePathProvider = CreateProvider(_filePathProviderQualifiedName);
		}

		void DelayExpensiveInit ()
		{
			if (_currentItems == null && Event.current.type == EventType.Repaint)
			{
				if (_readyToInit)
					_currentItems = _filePathProvider.GetItems(_searchFilter);
				_readyToInit = true;
				Repaint();
			}
		}

		void OnGUI ()
		{
			InitIfNeeded ();
			DelayExpensiveInit ();

			Rect searchAreaRect = new Rect (0,position.height-kSearchBarHeight-kMargin, position.width, kSearchBarHeight);
			Rect listAreaRect = new Rect (0, kMargin, position.width, position.height - kSearchBarHeight - 3*kMargin);
			
			HandleKeyboard (); // must be before Search field so we get the key input first
			ListAreaRect (listAreaRect);
			SearchArea (searchAreaRect);
		}

		void OffsetSelection (int offset)
		{
			int index = _currentItems.IndexOf(_selectedItem);
			if (index >= 0)
			{
				index += offset;
			}

			index = Mathf.Clamp(index, 0, _currentItems.Count-1);
			Select (_currentItems[index]);
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
							CloseWindow(_selectedItem); // close window
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

		void CloseWindow (FilePathProviderItem selectedItem)
		{
			if (selectedItem != null)
			{
				string filePath;
				int lineNumber;
				if (_filePathProvider.GetFileAndLineNumber (selectedItem.UserData, out filePath, out lineNumber))
				{
					CodeEditorWindow.OpenWindowFor(filePath); 
				}
			}
			Close ();
		}

		void ListAreaRect (Rect rect)
		{
			// Background
			Rect scrollRect = new Rect(rect.x + kMargin, rect.y, rect.width - 2 * kMargin, rect.height);
			GUI.Label(scrollRect, GUIContent.none, GUI.skin.textField);

			if (_currentItems == null)
				return;

			// ScrollArea
			int firstItem = 0;
			int lastItem = _currentItems.Count-1;
			int styleBorder = -1;

			scrollRect = new RectOffset (styleBorder,styleBorder,styleBorder,styleBorder).Add (scrollRect);
			Rect contentRect = new Rect(0, 0, 1, kLineHeight * _currentItems.Count);

			Event evt = Event.current;
			_scrollPosition = GUI.BeginScrollView (scrollRect, _scrollPosition, contentRect);
			{
				for (int i=firstItem; i<=lastItem; i++)
				{
					Rect itemRect = new Rect (0, i*kLineHeight, position.width-2*kMargin, kLineHeight);
					
					switch (evt.type)
					{
						case EventType.Repaint:
							bool selected = _currentItems[i] == _selectedItem;
							s_Styles.resultsLabel.Draw (itemRect, _currentItems[i].DisplayText, false, selected, selected, true);
							break;
						case EventType.MouseDown:
							if (itemRect.Contains (evt.mousePosition))
								Select (_currentItems[i]);
							break;
					}
				}

			} GUI.EndScrollView ();

		}

		void Select (FilePathProviderItem name)
		{
			_selectedItem = name;
			Repaint ();
		}

		void FilterChanged ()
		{
			_currentItems = _filePathProvider.GetItems (_searchFilter);
			if (_currentItems.Count > 0)
			{
				if (_currentItems.IndexOf (_selectedItem) < 0)
					_selectedItem = _currentItems[0];
			}
			else
			{
				_selectedItem = null;
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
} // namespace 
