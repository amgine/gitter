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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;

	public class HookListItem : CustomListBoxItem<Hook>
	{
		public HookListItem(Hook hook)
			: base(hook)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.Deleted += OnDeleted;
			DataContext.Revived += OnRevived;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.Deleted -= OnDeleted;
			DataContext.Revived -= OnRevived;
			base.OnListBoxDetached();
		}

		private void OnDeleted(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		private void OnRevived(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
			=> throw new NotImplementedException();

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
			=> throw new NotImplementedException();
	}
}
