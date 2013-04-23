using System.Collections.Generic;
using CodeEditor.Text.UI.Unity.Engine;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup
{
	internal class CodeCompletionWindowInput
	{
		public CodeView m_CodeView; // Owner of the CodeCompletionWindow
		public IListItemProvider m_ItemProvider;
		public IListItemGUI m_ItemGUI;
		public System.Action<IListItem, int> m_OnSelectCallback;
		public int m_SelectedListIndex = -1;
	}

	internal class CodeCompletionWindow : EditorWindow
	{
		static CodeCompletionWindow s_Instance;
		public static CodeCompletionWindow Instance
		{
			get { return s_Instance; }
		}

		private class Styles
		{
			public GUIStyle background = "grey_border";
		}
		static Styles s_Styles;

		CodeCompletionWindowInput _input;
		Rect _activatorRect;
		RegionResizer _regionResizer;
		IntSetting _userWidth;
		IntSetting _userHeight;
		Vector2 _scrollPosition;

		// Layout
		const float k_Margin = 5f;

		float ItemHeight
		{
			get { return _input.m_ItemGUI.ItemHeight; }
		}

		List<IListItem> ListItems
		{
			get { return _input.m_ItemProvider.GetList(); }
		}

		public void OnEnable()
		{
			s_Instance = this;
		}

		public void OnDisable()
		{
			s_Instance._input.m_CodeView.SetKeyboardFocus(); // can close window on lost focus
			s_Instance = null;
		}

		internal static bool ShowAtPosition(Rect screenActivatorRect, CodeCompletionWindowInput input, ISettings settings)
		{
			if (s_Instance == null)
			{
				s_Instance = CreateInstance<CodeCompletionWindow>();
				s_Instance.hideFlags = HideFlags.HideAndDontSave;
			}
			s_Instance.Init(screenActivatorRect, input, settings);
			return true;
		}

		internal static void CloseList()
		{
			if (s_Instance != null)
				s_Instance.Close();
		}

		void Init(Rect screenActivatorRect, CodeCompletionWindowInput input, ISettings settings)
		{
			_input = input;
			_activatorRect = screenActivatorRect;
			_userWidth = settings.GetSetting("PopupUserWidth") as IntSetting ?? new IntSetting("PopupUserWidth", -1, settings);
			_userHeight = settings.GetSetting("PopupUserHeight") as IntSetting ?? new IntSetting("PopupUserHeight", -1, settings);

			ShowAsDropDown(screenActivatorRect, GetWindowSize());
			Repaint();
		}

		Vector2 GetWindowSize()
		{
			Vector2 userSize = new Vector2(_userWidth.Value, _userHeight.Value);
			if (userSize.x < 0)
				userSize.x = 200;
			if (userSize.y < 0)
				userSize.y = ListItems.Count * ItemHeight + 2 * k_Margin; // Auto fit

			return userSize;
		}

		public void OnGUI()
		{
			if (s_Styles == null)
				s_Styles = new Styles();

			Event evt = Event.current;
			var itemList = _input.m_ItemProvider.GetList();
			KeyboardHandling(itemList);
			ResizeHandling();
			DoList(itemList);

			// Background with 1 pixel border for Win that does not have a dropshadow
			if (evt.type == EventType.Repaint && Application.platform == RuntimePlatform.WindowsEditor)
				s_Styles.background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
		}

		void KeyboardHandling (List<IListItem> items)
		{
			Event evt = Event.current;

			if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Escape)
				Cancel();

			if (evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
				UseSelectedWord();

			if (Event.current.type == EventType.KeyDown)
			{
				int offset = 0;
				switch (Event.current.keyCode)
				{
					case KeyCode.DownArrow:
						offset = 1;
						break;
					case KeyCode.UpArrow:
						offset = -1;
						break;
				}
				if (offset != 0)
				{
					if (_input.m_SelectedListIndex < 0 && offset < 0)
						Select(items.Count - 1);
					else
						Select((_input.m_SelectedListIndex + offset) % items.Count);
					Event.current.Use();
				}
				else
				{
					const bool requireKeyboardFocus = false;
					_input.m_CodeView.HandleKeyboard(requireKeyboardFocus);
					_input.m_CodeView.Repaint();
				}
			}
		}

		void DoList(List<IListItem> items)
		{
			Rect scrollRect = new Rect(0,0,position.width, position.height); 
			Rect contentRect = new Rect(0, 0, 1, ItemHeight * items.Count);
			float scrollBarWidth = 15f;
			float visibleWidth = position.width - (contentRect.height > scrollRect.height ? scrollBarWidth : 0f);
			_scrollPosition = GUI.BeginScrollView(scrollRect, _scrollPosition, contentRect);
			{
				for (int i = 0; i < items.Count; ++i)
				{
					IListItem item = items[i];
					bool selected = i == _input.m_SelectedListIndex;
					Rect itemRect = new Rect(0, i * ItemHeight, visibleWidth, ItemHeight);
					_input.m_ItemGUI.OnGUI(itemRect, item, selected);
					HandleListSelection(i);
				}
			}
			GUI.EndScrollView();
		}

		void HandleListSelection(int itemIndex)
		{
			Event evt = Event.current;
			Rect rect = new Rect(0, k_Margin + itemIndex * ItemHeight, position.width, ItemHeight);
			// Selection logic
			switch (evt.type)
			{
				case EventType.MouseDown:
					{
						if (Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
						{
							Select(itemIndex);
							evt.Use();
						}
					}
					break;
				case EventType.MouseUp:
					{
						evt.Use();
						if (Event.current.clickCount == 2)
							UseSelectedWord();
					}
					break;
			}		
		}

		void Cancel()
		{
			Event.current.Use();
			Close();
			GUIUtility.ExitGUI();
		}

		void Select(int index)
		{
			Event.current.Use();
			_input.m_SelectedListIndex = index;
		}

		void UseSelectedWord ()
		{
			Event.current.Use();
			var itemList = _input.m_ItemProvider.GetList();
			int index = _input.m_SelectedListIndex;
			if (index >= 0 && index < itemList.Count)
			{
				if (_input.m_OnSelectCallback != null)
					_input.m_OnSelectCallback(itemList[index], index);

				Close();
			}
		}

		void ResizeHandling()
		{
			if (_regionResizer == null)
			{
				const float borderWidth = 5f;
				_regionResizer = new RegionResizer(borderWidth, GetActiveBorders());
			}

			Rect region = position;
			Rect newRegion;
			if (_regionResizer.HandleResizing(region, new Vector2(60, 40), new Vector2(1000, 2000), out newRegion))
			{
				maxSize = minSize = new Vector2(newRegion.width, newRegion.height);
				position = newRegion;
				_userWidth.Value = (int)newRegion.width;
				_userHeight.Value = (int)newRegion.height;
			}
		}

		RegionResizer.BorderLocation[] GetActiveBorders()
		{
			bool belowText = _activatorRect.y < position.y;

			var activeBorders = new List<RegionResizer.BorderLocation>();
			activeBorders.Add(RegionResizer.BorderLocation.MiddleRight);
			if (belowText)
			{
				activeBorders.Add(RegionResizer.BorderLocation.Bottom);
				activeBorders.Add(RegionResizer.BorderLocation.BottomRight);
			}
			else // popup is above text
			{
				activeBorders.Add(RegionResizer.BorderLocation.Top);
				activeBorders.Add(RegionResizer.BorderLocation.TopRight);
			}
			
			return activeBorders.ToArray();
		}




		

	}
}
