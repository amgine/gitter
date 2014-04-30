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

namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class RevisionToolTip : CustomToolTip
	{
		private static readonly int MaxWidth = (int)((SystemInformation.SmallIconSize.Width / 16.0) * 450);

		private RevisionHeaderContent _content;

		public RevisionToolTip()
		{
			_content = new RevisionHeaderContent();
			_content.Style = GitterApplication.DefaultStyle;
		}

		public Revision Revision
		{
			get { return _content.Revision; }
			set { _content.Revision = value; }
		}

		protected override void OnPaint(DrawToolTipEventArgs e)
		{
			base.OnPaint(e);
			_content.OnPaint(e.Graphics, e.Bounds);
		}

		public override Size Size
		{
			get
			{
				var size = _content.OnMeasure(GraphicsUtility.MeasurementGraphics, MaxWidth);
				size.Height += 3;
				return size;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				_content.Revision = null;
			}
			base.Dispose(disposing);
		}
	}
}
