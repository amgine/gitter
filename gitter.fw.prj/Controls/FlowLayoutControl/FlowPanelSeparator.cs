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

	/// <summary>Panel for separating other panels.</summary>
	public class FlowPanelSeparator : FlowPanel
	{
		private int _height;
		private FlowPanelSeparatorStyle _style;

		/// <summary>Create <see cref="FlowPanelSeparator"/>.</summary>
		public FlowPanelSeparator()
		{
			_height = 16;
		}

		/// <summary>Separator height.</summary>
		public int Height
		{
			get { return _height; }
			set
			{
				if(_height != value)
				{
					_height = value;
				}
			}
		}

		public FlowPanelSeparatorStyle SeparatorStyle
		{
			get { return _style; }
			set
			{
				if(_style != value)
				{
					_style = value;
				}
			}
		}

		protected override Size OnMeasure(FlowPanelMeasureEventArgs measureEventArgs)
		{
			return new Size(0, _height);
		}

		protected override void OnPaint(FlowPanelPaintEventArgs paintEventArgs)
		{
			var graphics = paintEventArgs.Graphics;
			var rect = paintEventArgs.Bounds;
			switch(_style)
			{
				case FlowPanelSeparatorStyle.Line:
					{
						var y = _height / 2;
						var x = y;
						var w = Math.Max(FlowControl.ContentSize.Width, FlowControl.ContentArea.Width) - 2 * x;
						if(w > 0)
						{
							x += rect.X;
							y += rect.Y;
							graphics.DrawLine(Pens.Gray, x, y, x + w, y);
						}
					}
					break;
			}
		}
	}
}
