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
	using System.Collections.Generic;
	using System.Linq;
	using System.Drawing;
	using System.Windows.Forms;

	public sealed class MSVS2012ButtonRenderer : CustomButtonRenderer
	{
		#region Color Tables

		public interface IColorTable
		{
			Color Border { get; }
			Color Background { get; }
			Color Foreground { get; }

			Color HoverBorder { get; }
			Color HoverBackground { get; }

			Color PressedBorder { get; }
			Color PressedBackground { get; }

			Color DisabledBorder { get; }
			Color DisabledBackground { get; }
			Color DisabledForeground { get; }
		}

		private sealed class DarkColorTable : IColorTable
		{
			public Color Border
			{
				get { return Color.FromArgb(84, 84, 92); }
			}

			public Color Background
			{
				get { return Color.FromArgb(63, 63, 70); }
			}

			public Color Foreground
			{
				get { return MSVS2012DarkColors.WINDOW_TEXT; }
			}

			public Color HoverBorder
			{
				get { return Color.FromArgb(106, 106, 117); }
			}

			public Color HoverBackground
			{
				get { return Color.FromArgb(84, 84, 92); }
			}

			public Color PressedBorder
			{
				get { return Color.FromArgb(28, 151, 234); }
			}

			public Color PressedBackground
			{
				get { return Color.FromArgb(0, 122, 204); }
			}

			public Color DisabledBorder
			{
				get { return Color.FromArgb(67, 67, 70); }
			}

			public Color DisabledBackground
			{
				get { return Color.FromArgb(37, 37, 38); }
			}

			public Color DisabledForeground
			{
				get { return Color.FromArgb(109, 109, 109); }
			}
		}

		private static IColorTable _darkColors;

		public static IColorTable DarkColors
		{
			get
			{
				if(_darkColors == null)
				{
					_darkColors = new DarkColorTable();
				}
				return _darkColors;
			}
		}

		#endregion

		#region Data

		private readonly IColorTable _colorTable;

		#endregion

		#region .ctor

		public MSVS2012ButtonRenderer(IColorTable colorTable)
		{
			Verify.Argument.IsNotNull(colorTable, "colorTable");

			_colorTable = colorTable;
		}

		#endregion

		#region Properties

		private IColorTable ColorTable
		{
			get { return _colorTable; }
		}

		#endregion

		#region Methods

		public override void Render(Graphics graphics, Rectangle clipRectangle, CustomButton button)
		{
			Color border;
			Color background;
			Color foreground;
			if(button.Enabled)
			{
				if(button.IsPressed)
				{
					border		= ColorTable.PressedBorder;
					background	= ColorTable.PressedBackground;
					foreground	= ColorTable.Foreground;
				}
				else if(button.Focused || button.IsMouseOver)
				{
					border		= ColorTable.HoverBorder;
					background	= ColorTable.HoverBackground;
					foreground	= ColorTable.Foreground;
				}
				else
				{
					border		= ColorTable.Border;
					background	= ColorTable.Background;
					foreground	= ColorTable.Foreground;
				}
			}
			else
			{
				border		= ColorTable.DisabledBorder;
				background	= ColorTable.DisabledBackground;
				foreground	= ColorTable.DisabledForeground;
			}
			using(var brush = new SolidBrush(background))
			{
				graphics.FillRectangle(brush, clipRectangle);
			}
			var bounds = new Rectangle(Point.Empty, button.Size);
			TextRenderer.DrawText(graphics, button.Text, button.Font, bounds, foreground);
			using(var pen = new Pen(border))
			{
				bounds.Width -= 1;
				bounds.Height -= 1;
				graphics.DrawRectangle(pen, bounds);
			}
		}

		#endregion
	}
}
