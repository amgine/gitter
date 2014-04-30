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

	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	sealed class RepositoryGroupListItem : CustomListBoxItem<RepositoryGroup>
	{
		private RepositoryGroupBinding _binding;

		public RepositoryGroupListItem(RepositoryGroup repositoryGroup)
			: base(repositoryGroup)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();
			DataContext.NameChanged += OnRepositoryGroupNameChanged;
			if(_binding != null)
			{
				_binding.Dispose();
			}
			_binding = new RepositoryGroupBinding(this.Items, DataContext);
		}

		protected override void OnListBoxDetached()
		{
			DataContext.NameChanged -= OnRepositoryGroupNameChanged;
			if(_binding != null)
			{
				_binding.Dispose();
			}
			base.OnListBoxDetached();
		}

		private void OnRepositoryGroupNameChanged(object sender, EventArgs e)
		{
			InvalidateSafe();
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			Assert.IsNotNull(measureEventArgs);

			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureText(DataContext.Name);

				default:
					return base.MeasureSubItem(measureEventArgs);
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintText(DataContext.Name);
					break;
			}
		}
	}
}
