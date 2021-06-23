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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	public class DpiBindings
	{
		private readonly Dictionary<ToolStripItem, IDpiBoundValue<Bitmap>> _bindings = new();

		public DpiBindings(Control toolStrip)
		{
			Verify.Argument.IsNotNull(toolStrip, nameof(toolStrip));

			Control = toolStrip;
			if(!Control.IsDisposed)
			{
				Control.Disposed               += OnControlDisposed;
				Control.DpiChangedBeforeParent += OnControlDpiChangedBeforeParent;
			}
		}

		private void OnControlDpiChangedBeforeParent(object sender, EventArgs e)
		{
			var dpi = new Dpi(Control.DeviceDpi);
			foreach(var binding in _bindings)
			{
				binding.Key.Image = binding.Value.GetValue(dpi);
			}
		}

		private void OnControlDisposed(object sender, EventArgs e)
		{
			Control.Disposed               -= OnControlDisposed;
			Control.DpiChangedBeforeParent -= OnControlDpiChangedBeforeParent;
			_bindings.Clear();
		}

		private Control Control { get; }

		public void BindImage(ToolStripItem item, IDpiBoundValue<Bitmap> image)
		{
			Verify.Argument.IsNotNull(item, nameof(item));
			Verify.State.IsFalse(Control.IsDisposed);

			if(image is not null)
			{
				_bindings[item] = image;
				item.Image = image.GetValue(new Dpi(Control.DeviceDpi));
			}
			else
			{
				_bindings.Remove(item);
				item.Image = default;
			}
		}
	}
}
