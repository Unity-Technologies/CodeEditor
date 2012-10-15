using System.Collections.Generic;
using CodeEditor.Text.UI.Unity.Engine.Implementation;
using UnityEditor;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CodeViewPopUp : EditorWindow
	{
		static CodeViewPopUp s_Instance;

		private class Styles
		{
			public GUIStyle label = "PR Label";
			public GUIStyle background = "grey_border";
			public GUIStyle autocompleteOverlay = "grey_border"; // TODO: create a specific style for this
		}
		static Styles s_Styles;

		public class PopUpState
		{
			public List<string> m_ListElements;
			public System.Action<string, int> m_OnSelectCallback;	// displayText, index into m_ListElements
			public int m_SelectedCompletion = -1;					// starts invisible
			public CodeView m_CodeView;

			public PopUpState ()
			{
				m_ListElements = new List<string>();
			}
		}

		// State
		PopUpState m_State;
		Vector2 m_ScreenPos;
		int m_MouseHoverIndex = -1;

		// Layout
		const float k_LineHeight = 16f;
		const float k_Margin = 5f;

		CodeViewPopUp ()
		{
		}

		void OnEnable()
		{
		}

		void OnDisable ()
		{
			s_Instance = null;
		}

		internal static bool ShowAtPosition (Vector2 pos, PopUpState popUpState)
		{
			if (s_Instance == null) {
				s_Instance = CreateInstance<CodeViewPopUp>();
				s_Instance.hideFlags = HideFlags.HideAndDontSave;
			}
			s_Instance.Init (pos, popUpState);
			return true;
		}

		internal static void CloseList ()
		{
			if (s_Instance != null)
				s_Instance.Close ();
		}

		void Init (Vector2 pos, PopUpState popUpState)
		{
			m_State = popUpState;
			m_ScreenPos = GUIUtility.GUIToScreenPoint(pos);
			Rect buttonRect = new Rect (m_ScreenPos.x, m_ScreenPos.y - 16, 16, 16); // fake a button: we know we are showing it below the bottonRect if possible
			ShowAsDropDown (buttonRect, GetWindowSize ());
		}

		Vector2 GetWindowSize ()
		{
			return new Vector2 (150f, m_State.m_ListElements.Count * k_LineHeight + 2 * k_Margin);
		}

		internal void OnGUI ()
		{
			Event evt = Event.current;
			// We do not use the layout event
			if (evt.type == EventType.layout)
				return;

			if (s_Styles == null)
				s_Styles = new Styles ();

			if (evt.type == EventType.KeyDown && evt.keyCode == KeyCode.Escape)
				Cancel ();

			if (evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
				Select (m_State.m_SelectedCompletion);

			KeyboardHandling ();
			DoList ();

			// Background with 1 pixel border for Win that does not have a dropshadow
			if ( evt.type == EventType.Repaint && Application.platform == RuntimePlatform.WindowsEditor)
				s_Styles.background.Draw(new Rect(0, 0, position.width, position.height), false, false, false, false);
		}

		void KeyboardHandling ()
		{
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
					if (m_State.m_SelectedCompletion < 0 && offset < 0)
						m_State.m_SelectedCompletion = m_State.m_ListElements.Count - 1;
					else
						m_State.m_SelectedCompletion = (m_State.m_SelectedCompletion + offset) % m_State.m_ListElements.Count;
					Event.current.Use ();
				}
				else
				{
					bool requireKeyboardFocus = false;
					m_State.m_CodeView.HandleKeyboard (requireKeyboardFocus);
					m_State.m_CodeView.Repaint ();
				}
			}
		}

		void Cancel ()
		{
			Close();
			GUIUtility.ExitGUI();
		}

		void Select (int index)
		{
			if (index >= 0 && index < m_State.m_ListElements.Count)
			{
				if (m_State.m_OnSelectCallback != null)
					m_State.m_OnSelectCallback (m_State.m_ListElements[index], index);
			}
			Event.current.Use();

			// Auto close on selection
			Close ();
		}



		void DoList ()
		{
			Repaint (); // force repaint for hover effect

			Event evt = Event.current;
			for (int i=0; i<m_State.m_ListElements.Count; ++i)
			{
				string element = m_State.m_ListElements [i];

				Rect rect = new Rect(0, k_Margin + i * k_LineHeight, position.width, k_LineHeight);

				if (rect.Contains (Event.current.mousePosition) && m_MouseHoverIndex != i)
				{
					m_MouseHoverIndex = i;
					m_State.m_SelectedCompletion = i;
				}

				switch (evt.type)
				{
					case EventType.Repaint:
					{
						GUIStyle style = s_Styles.label;
						style.padding.left = (int)k_Margin;

						bool selected = false;
						bool focused = MissingEditorAPI.ParentHasFocus(this);
						bool isHover = false;
						bool isActive = selected;

						style.Draw(rect, MissingEngineAPI.GUIContent_Temp (element), isHover, isActive, selected, focused);


						// DoGUI a border around the current focused item
						if (i == m_State.m_SelectedCompletion)
						{
							rect.x+=2;
							rect.width-=4;
							s_Styles.autocompleteOverlay.Draw(rect, false, false, false, false);
						}
					}
						break;
					case EventType.MouseDown:
					{
						if (Event.current.button == 0 && rect.Contains (Event.current.mousePosition))
						{
							m_State.m_SelectedCompletion = i;
							evt.Use ();
						}
					}
						break;
					case EventType.MouseUp:
					{
						if (Event.current.button == 0 && rect.Contains (Event.current.mousePosition) && m_State.m_SelectedCompletion == i)
						{
							Select (i);
							evt.Use ();
						}
					}
						break;
				}
			}
		}

	}
}
