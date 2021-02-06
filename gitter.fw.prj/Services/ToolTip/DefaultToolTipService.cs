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

namespace gitter.Framework.Services
{
	using System;
	using System.Windows.Forms;

	public sealed class DefaultToolTipService : IToolTipService, IDisposable
	{
		private ToolTip _toolTip;

		public DefaultToolTipService()
		{
			_toolTip = new ToolTip()
			{
				/*IsBalloon = true,*/
			};
		}

		public void Register(Control control, string text)
			=> _toolTip.SetToolTip(control, text);

		public void Unregister(Control control)
			=> _toolTip.SetToolTip(control, string.Empty);

		~DefaultToolTipService() => Dispose(disposing: false);

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_toolTip != null)
				{
					_toolTip.RemoveAll();
					_toolTip.Dispose();
					_toolTip = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
