﻿#region Copyright Notice
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
	using System.Windows.Forms;

	public class ViewMenuItem : ToolStripMenuItem
	{
		private readonly IWorkingEnvironment _environment;
		private readonly IViewFactory _factory;
		private ToolStrip _owner;

		public ViewMenuItem(IViewFactory factory, IWorkingEnvironment environment)
			: base(factory.Name, null, OnClick)
		{
			_factory = factory;
			_environment = environment;
		}

		protected override void OnOwnerChanged(EventArgs e)
		{
			if(_factory.ImageProvider is not null)
			{
				if(_owner is not null)
				{
					_owner.DpiChangedAfterParent -= OnOwnerDpiChangedAfterParent;
					_owner = null;
				}
				if(Owner is null) Image = null;
				else
				{
					UpdateImage();
					_owner = Owner;
					_owner.DpiChangedAfterParent += OnOwnerDpiChangedAfterParent;
				}
			}
			base.OnOwnerChanged(e);
		}

		private void OnOwnerDpiChangedAfterParent(object sender, EventArgs e) => UpdateImage();

		private void UpdateImage()
		{
			var iconSize = new DpiConverter(Owner).ConvertX(16);
			Image = _factory.ImageProvider.GetImage(iconSize);
		}

		private static void OnClick(object sender, EventArgs e)
		{
			var item = (ViewMenuItem)sender;
			item._environment.ViewDockService.ShowView(item._factory.Guid, activate: true);
		}
	}
}
