using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;
using CodeEditor.Reactive;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	public partial class NavigatorWindow : EditorWindow
	{
		internal static Func<INavigateToItemProviderAggregator> ProviderAggregatorFactory;
	
		private const float kSearchBarHeight = 17;
		private const float kMargin = 10f;
		private const float kLineHeight = 16f;

		[NonSerialized] private INavigateToItemProvider _navigateToItemProvider;
		[NonSerialized] private List<INavigateToItem> _currentItems;
		[NonSerialized] private IDisposable _searchSubscription;
		[NonSerialized] private INavigateToItem _selectedItem;
		private string _searchFilter = "";
		private Vector2 _scrollPosition;

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
		private UnityEditorScheduler _unityScheduler;

		public static void Open()
		{
			var window = GetWindow<NavigatorWindow>();
			window.title = "Navigate To";
		}

		private void InitIfNeeded()
		{
			if(s_Styles == null)
				s_Styles = new Styles();

			if (_navigateToItemProvider == null)
				_navigateToItemProvider = ProviderAggregatorFactory();
		}

		private void DelayExpensiveInit()
		{
			if(_currentItems == null && Event.current.type == EventType.Repaint)
			{
				if(_readyToInit)
					StartSearch();
				_readyToInit = true;
				Repaint();
			}
		}

		private void StartSearch()
		{
			// TODO: use SerialDisposable instead
			if (_searchSubscription != null) _searchSubscription.Dispose();
			_selectedItem = null;
			_currentItems = new List<INavigateToItem>();
			_searchSubscription = _navigateToItemProvider.Search(_searchFilter).ObserveOnThreadPool().Subscribe(OnNextItem);
		}

		private void OnNextItem(INavigateToItem item)
		{
			UnityEditorScheduler.Instance.Schedule(() =>
			{
				if (_selectedItem == null)
					_selectedItem = item;
				_currentItems.Add(item);
				Repaint();
			});
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

		private void CloseWindow(INavigateToItem selectedItem)
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

		private void Select(INavigateToItem name)
		{
			_selectedItem = name;
			Repaint();
		}

		private void FilterChanged()
		{
			StartSearch();
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
