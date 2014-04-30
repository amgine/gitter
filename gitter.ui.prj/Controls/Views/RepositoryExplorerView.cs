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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Properties.Resources;

	public partial class RepositoryExplorerView : ViewBase
	{
		public RepositoryExplorerView(IWorkingEnvironment environment)
			: base(Guids.RepositoryExplorerView, environment)
		{
			InitializeComponent();

			Text = Resources.StrRepositoryExplorer;
		}

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRepositoryExplorer"]; }
		}

		private void OnItemActivated(object sender, ItemEventArgs e)
		{
			var item = e.Item as ViewListItem;
			if(item != null)
			{
				WorkingEnvironment.ViewDockService.ShowView(item.DataContext.Guid);
			}
		}

		public void AddItem(CustomListBoxItem item)
		{
			_lstRepositoryExplorer.Items.Add(item);
		}

		public void RemoveItem(CustomListBoxItem item)
		{
			_lstRepositoryExplorer.Items.Remove(item);
		}
	}
}
