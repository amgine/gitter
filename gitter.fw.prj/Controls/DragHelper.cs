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
	using System.Windows.Forms;

	public sealed class DragHelper
	{
		private int _x;
		private int _y;

		public void Start(Point point) => Start(point.X, point.Y);

		public void Start(int x, int y)
		{
			Verify.State.IsFalse(IsTracking);

			_x = x;
			_y = y;
			IsTracking = true;
		}

		public void Stop()
		{
			Verify.State.IsTrue(IsTracking);

			IsTracking = false;
			IsDragging = false;
		}

		public bool Update(Point point) => Update(point.X, point.Y);

		public bool Update(int x, int y)
		{
			Verify.State.IsTrue(IsTracking);

			if(IsDragging) return true;
			var dragSize = SystemInformation.DragSize;
			IsDragging =
				(Math.Abs(x - _x) * 2 > dragSize.Width) ||
				(Math.Abs(y - _y) * 2 > dragSize.Height);
			return IsDragging;
		}

		public bool IsTracking { get; private set; }

		public bool IsDragging { get; private set; }
	}
}
