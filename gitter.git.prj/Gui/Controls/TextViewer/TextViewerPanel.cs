namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	class TextViewerPanel<T> : FlowPanel
		where T : TextLineBase
	{
		private T[] _lines;

		public TextViewerPanel(T[] lines)
		{
			if(lines == null) throw new ArgumentNullException("lines");

			_lines = lines;
		}

		public T this[int index]
		{
			get { return _lines[index]; }
		}

		public int Count
		{
			get { return _lines.Length; }
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
		}
	}
}
