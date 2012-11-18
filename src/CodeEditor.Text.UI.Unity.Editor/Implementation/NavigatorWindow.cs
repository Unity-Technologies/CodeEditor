using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public partial class NavigatorWindow : EditorWindow
	{
		private const float kSearchBarHeight = 17;
		private const float kMargin = 10f;
		private const float kLineHeight = 16f;

		[NonSerialized] private INavigatorWindowItemProvider _filePathProvider;
		[NonSerialized] private List<INavigatorWindowItem> _currentItems;
		[NonSerialized] private INavigatorWindowItem _selectedItem;
		private string _searchFilter = "";
		private Vector2 _scrollPosition;

		private string _filePathProviderQualifiedName;
		               // stored as string so we can serialize and reconstruct a filePathProvider after domain reloads

		[NonSerialized] private bool _readyToInit = false;

		private class Styles
		{
			public GUIStyle resultsLabel = new GUIStyle("PR Label");

			public Styles()
			{
				resultsLabel.padding.left = 5;
			}
		}

		private static Styles s_Styles;


		public static void Open(Type filePathProviderType)
		{
			if(!typeof(INavigatorWindowItemProvider).IsAssignableFrom(filePathProviderType))
				throw new ArgumentException("Invalid Type as argument: " + filePathProviderType + " cannot be assigned to " +
				                            typeof(INavigatorWindowItemProvider));

			var window = GetWindow<NavigatorWindow>();
			window.title = "Navigate To";
			window._filePathProviderQualifiedName = filePathProviderType.AssemblyQualifiedName;
		}

		private static INavigatorWindowItemProvider CreateProvider(string assemblyQualifiedName)
		{
			INavigatorWindowItemProvider provider = null;
			Type t = Type.GetType(assemblyQualifiedName);
			if(t != null)
				provider = Activator.CreateInstance(t) as INavigatorWindowItemProvider;

			if(provider == null)
				Debug.LogError("Could not create a FilePathProvider from " + assemblyQualifiedName);
			return provider;
		}

		private void InitIfNeeded()
		{
			if(s_Styles == null)
				s_Styles = new Styles();

			if(_filePathProvider == null)
				_filePathProvider = CreateProvider(_filePathProviderQualifiedName);
		}

		private void DelayExpensiveInit()
		{
			if(_currentItems == null && Event.current.type == EventType.Repaint)
			{
				if(_readyToInit)
					_currentItems = _filePathProvider.Search(_searchFilter);
				_readyToInit = true;
				Repaint();
			}
		}

		private void OnGUI()
		{
			InitIfNeeded();
			DelayExpensiveInit();

			Rect searchAreaRect = new Rect(0, position.height - kSearchBarHeight - kMargin, position.width, kSearchBarHeight);
			Rect listAreaRect = new Rect(0, kMargin, position.width, position.height - kSearchBarHeight - 3 * kMargin);

			HandleKeyboard(); // must be before Search field so we get the key input first
			ListAreaRect(listAreaRect);
			SearchArea(searchAreaRect);
		}

		private void OffsetSelection(int offset)
		{
			int index = _currentItems.IndexOf(_selectedItem);
			if(index >= 0)
			{
				index += offset;
			}

			index = Mathf.Clamp(index, 0, _currentItems.Count - 1);
			Select(_currentItems[index]);
		}

		private void HandleKeyboard()
		{
			Event evt = Event.current;
			switch(evt.type)
			{
				case EventType.KeyDown:
					switch(evt.keyCode)
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

		private void CloseWindow(INavigatorWindowItem selectedItem)
		{
			if(selectedItem != null)
			{
				selectedItem.NavigateTo();
			}
			Close();
		}

		private void ListAreaRect(Rect rect)
		{
			// Background
			Rect scrollRect = new Rect(rect.x + kMargin, rect.y, rect.width - 2 * kMargin, rect.height);
			GUI.Label(scrollRect, GUIContent.none, GUI.skin.textField);

			if(_currentItems == null)
				return;

			// ScrollArea
			int firstItem = 0;
			int lastItem = _currentItems.Count - 1;
			int styleBorder = -1;

			scrollRect = new RectOffset(styleBorder, styleBorder, styleBorder, styleBorder).Add(scrollRect);
			Rect contentRect = new Rect(0, 0, 1, kLineHeight * _currentItems.Count);

			Event evt = Event.current;
			_scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, contentRect);
			{
				for(int i = firstItem; i <= lastItem; i++)
				{
					Rect itemRect = new Rect(0, i * kLineHeight, position.width - 2 * kMargin, kLineHeight);

					switch(evt.type)
					{
						case EventType.Repaint:
							bool selected = _currentItems[i] == _selectedItem;
							s_Styles.resultsLabel.Draw(itemRect, _currentItems[i].DisplayText, false, selected, selected, true);
							break;
						case EventType.MouseDown:
							if(evt.button == 0 && itemRect.Contains(evt.mousePosition))
							{
								if(evt.clickCount == 1)
									Select(_currentItems[i]);
								else if(evt.clickCount == 2)
									CloseWindow(_currentItems[i]);
							}
							break;
					}
				}
			}
			GUI.EndScrollView();
		}

		private void Select(INavigatorWindowItem name)
		{
			_selectedItem = name;
			Repaint();
		}

		private void FilterChanged()
		{
			_currentItems = _filePathProvider.Search(_searchFilter);
			if(_currentItems.Count > 0)
			{
				if(_currentItems.IndexOf(_selectedItem) < 0)
					_selectedItem = _currentItems[0];
			}
			else
			{
				_selectedItem = null;
			}
		}

		// This is our search field
		private void SearchArea(Rect rect)
		{
			//GUI.Label (rect, GUIContent.none, s_Styles.toolbarBack);
			Rect searchFieldRect = new Rect(rect.x + kMargin, rect.y, rect.width - 2 * kMargin, rect.height);
			EditorGUI.BeginChangeCheck();
			{
				GUI.SetNextControlName("SearchFilter");
				_searchFilter = GUI.TextField(searchFieldRect, _searchFilter);
			}
			if(EditorGUI.EndChangeCheck())
				FilterChanged();

			GUI.FocusControl("SearchFilter");
		}
	}
}

// namespace 
