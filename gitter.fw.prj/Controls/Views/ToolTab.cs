#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

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
