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

	internal struct GripBounds
	{
		private const int GripSize = 6;
		private const int CornerGripSize = GripSize << 1;

		private readonly Rectangle _clientRectangle;

		public GripBounds(Rectangle clientRectangle)
		{
			_clientRectangle = clientRectangle;
		}

		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		public Rectangle Bottom
		{
			get
			{
				var rect = _clientRectangle;
				rect.Y = rect.Bottom - GripSize + 1;
				rect.Height = GripSize;
				return rect;
			}
		}

		public Rectangle BottomRight
		{
			get
			{
				var rect = _clientRectangle;
				rect.Y = rect.Bottom - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Top
		{
			get
			{
				var rect = _clientRectangle;
				rect.Height = GripSize;
				return rect;
			}
		}

		public Rectangle TopRight
		{
			get
			{
				var rect = _clientRectangle;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Left
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = GripSize;
				return rect;
			}
		}

		public Rectangle BottomLeft
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = CornerGripSize;
				rect.Y = rect.Height - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				return rect;
			}
		}

		public Rectangle Right
		{
			get
			{
				var rect = _clientRectangle;
				rect.X = rect.Right - GripSize + 1;
				rect.Width = GripSize;
				return rect;
			}
		}

		public Rectangle TopLeft
		{
			get
			{
				var rect = _clientRectangle;
				rect.Width = CornerGripSize;
				rect.Height = CornerGripSize;
				return rect;
			}
		}
	}
}
