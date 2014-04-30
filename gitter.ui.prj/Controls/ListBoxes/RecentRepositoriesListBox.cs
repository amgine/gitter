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

namespace gitter
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	internal sealed class RecentRepositoriesListBox : CustomListBox
	{
		private readonly DragHelper _dragHelper;

		public RecentRepositoriesListBox()
		{
			Columns.Add(new CustomListBoxColumn(0, Resources.StrName, true)
			{
				SizeMode = ColumnSizeMode.Fill
			});

			HeaderStyle = HeaderStyle.Hidden;
			_dragHelper = new DragHelper();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left && SelectedItems.Count != 0)
			{
				_dragHelper.Start(e.X, e.Y);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if(_dragHelper.IsTracking)
			{
				_dragHelper.Stop();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(_dragHelper.IsTracking && _dragHelper.Update(e.X, e.Y))
			{
				if(SelectedItems.Count != 1) return;
				var item = SelectedItems[0];
				using(var dragImage = RepositoryDragImage.Create(
					((RecentRepositoryListItem)item).DataContext.Path))
				{
					dragImage.ShowDragVisual(this);
					DoDragDrop(item, DragDropEffects.Copy);
				}
				_dragHelper.Stop();
			}
		}
	}
}
