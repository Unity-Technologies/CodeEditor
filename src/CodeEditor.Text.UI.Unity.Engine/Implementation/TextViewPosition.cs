using System;
using UnityEngine;

namespace CodeEditor.Text.UI.Unity.Engine.Implementation
{
	public struct TextViewPosition 
	{
		public TextViewPosition (int Column, int Row)
		{
			column = Column;
			row = Row;
		}
		public int column, row;

		public override string ToString ()
		{
			return string.Format ("(row {0}, column {1})", row, column);
		}
	}
}
