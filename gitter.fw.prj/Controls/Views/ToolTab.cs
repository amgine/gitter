namespace gitter.Framework.Controls.Tools
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	sealed class ToolTab
	{
		#region Data

		private readonly Tool _tool;
		private readonly Orientation _orientation;
		private readonly AnchorStyles _anchor;
		private int _offset;

		#endregion

		#region .ctor

		public ToolTab(Tool tool, AnchorStyles anchor, Orientation orientation)
		{
			if(tool == null) throw new ArgumentNullException("tool");

			_tool = tool;
			_anchor = anchor;
			_orientation = orientation;
		}

		#endregion

		public Tool Tool
		{
			get { return _tool; }
		}

		public AnchorStyles Anchor
		{
			get { return _anchor; }
		}

		public Orientation Orientation
		{
			get { return _orientation; }
		}

		public int Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}
	}
}
