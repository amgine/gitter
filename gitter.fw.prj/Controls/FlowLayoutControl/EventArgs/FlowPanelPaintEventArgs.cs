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

namespace gitter.Framework.Controls
{
	using System;
	using System.Drawing;

	public class FlowPanelPaintEventArgs : EventArgs
	{
		#region Data

		private readonly Graphics _graphics;
		private readonly Rectangle _bounds;
		private readonly Rectangle _clipRectangle;

		#endregion

		#region .ctor

		public FlowPanelPaintEventArgs(Graphics graphics, Rectangle clipRectangle, Rectangle bounds)
		{
			_graphics = graphics;
			_clipRectangle = clipRectangle;
			_bounds = bounds;
		}

		#endregion

		#region Properties

		public Graphics Graphics
		{
			get { return _graphics; }
		}

		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		public Rectangle ClipRectangle
		{
			get { return _clipRectangle; }
		}

		#endregion
	}
}
