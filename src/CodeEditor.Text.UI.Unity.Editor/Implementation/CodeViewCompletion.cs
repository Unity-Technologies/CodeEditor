using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Text.Data;
using CodeEditor.Text.UI.Implementation;
using CodeEditor.Text.UI.Unity.Editor.Implementation.ListPopup;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;
using UnityEditor;

namespace CodeEditor.Text.UI.Unity.Editor.Implementation
{
	internal class CodeViewCompletion
	{
		enum State
		{
			Idle,
			UpdateScreenRectNeeded,
			ShowWindow
		};
		State _state = State.Idle;
		string _word;
		Rect _wordScreenRect;

		private readonly CodeView _codeView;		// owner
		private readonly ITextView _textView;
		private readonly ITextViewDocument _document;

		public CodeViewCompletion(CodeView codeView, ITextView textView)
		{
			_codeView = codeView;
			_textView = textView;
			_document = textView.Document;

			_document.Buffer.Changed += OnBufferChanged;
			_textView.TextViewEvent += OnTextViewEvent;
		}
		
		void OnBufferChanged(object sender, TextChangeArgs args)
		{
			_state = State.UpdateScreenRectNeeded;
			_codeView.Repaint();
		}

		void OnTextViewEvent()
		{
			UpdateScreenRectOfCurrentWord();
		}

		// Needs to be called inside textview's scrollview section to be able to properly convert to screen space (see TextViewEvent)
		void UpdateScreenRectOfCurrentWord()
		{
			if (_state == State.UpdateScreenRectNeeded && Event.current.type == EventType.Repaint)
			{
				_state = State.ShowWindow;

				var textSpan = _codeView.PreviousWordSpan();
				_word = textSpan.Text.Trim(new[] { '\n', ' ', '\t' });

				var row = _codeView.LineNumberForPosition(textSpan.Start);
				var column = textSpan.Start - _codeView.LineStart(row);
				var subRect = _textView.GetSubstringRect(row, column, textSpan.Length);
				_wordScreenRect = GUIToScreenRect(subRect);
			}
		}

		public void OnGUI()
		{
			if (_state == State.ShowWindow)
			{
				_state = State.Idle; 

				if (_word.Length == 0)
				{
					CodeCompletionWindow.CloseList();
					return;
				}

				var input = new CodeCompletionWindowInput();
				var provider = new CodeCompletionListItemProvider(_word);
				input.m_ItemProvider = provider;
				input.m_ItemGUI = new CodeCompletionListItemGUI(18, provider);
				input.m_OnSelectCallback += CodeCompletionCallback;
				input.m_SelectedListIndex = 0; // Use -1 for invisible marker when showing
				input.m_CodeView = _codeView;

				CodeCompletionWindow.ShowAtPosition(_wordScreenRect, input, _textView.Settings);
			}
		}

		void CodeCompletionCallback(IListItem selectedItem, int selectedIndex)
		{
			var item = selectedItem as CodeCompletionListItem;

			TextSpan curWord = _codeView.PreviousWordSpan();
			int delta = item.Text.Length - curWord.Length;
			int newColumn = _codeView.Caret.Column + delta;

			_document.Delete(curWord.Start, curWord.Length);
			_document.Insert(curWord.Start, item.Text);
			_codeView.Caret.SetPosition(_codeView.Caret.Row, newColumn);
			_codeView.SetKeyboardFocus();
		}

		static Rect GUIToScreenRect(Rect guiRect)
		{
			Vector2 screenPoint = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.x, guiRect.y));
			guiRect.x = screenPoint.x;
			guiRect.y = screenPoint.y;
			return guiRect;
		}


	}
}

