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

namespace gitter.TeamCity.Gui
{
	using System;
	using System.Globalization;
	using System.Drawing;

	using gitter.Framework.Controls;

	sealed class BuildTypeListItem : CustomListBoxItem<BuildType>
	{
		private static readonly Image ImgIcon = CachedResources.Bitmaps["ImgBuildType"];

		public BuildTypeListItem(BuildType buildType)
			: base(buildType)
		{
		}

		protected override void OnListBoxAttached()
		{
			base.OnListBoxAttached();

			DataContext.PropertyChanged += OnBuildTypePropertyChanged;
		}

		protected override void OnListBoxDetached()
		{
			DataContext.PropertyChanged -= OnBuildTypePropertyChanged;

			base.OnListBoxDetached();
		}

		private void OnBuildTypePropertyChanged(object sender, TeamCityObjectPropertyChangedEventArgs e)
		{
			if(e.Property == BuildType.NameProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Name);
			}
			else if(e.Property == BuildType.IdProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.Id);
			}
			else if(e.Property == BuildType.WebUrlProperty)
			{
				InvalidateSubItemSafe((int)ColumnId.WebUrl);
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch((ColumnId)measureEventArgs.SubItemId)
			{
				case ColumnId.Id:
					return measureEventArgs.MeasureText(DataContext.Id);
				case ColumnId.Name:
					return measureEventArgs.MeasureImageAndText(ImgIcon, DataContext.Name);
				case ColumnId.WebUrl:
					return measureEventArgs.MeasureText(DataContext.WebUrl);
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Id:
					paintEventArgs.PaintText(DataContext.Id);
					break;
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(ImgIcon, DataContext.Name);
					break;
				case ColumnId.WebUrl:
					paintEventArgs.PaintText(DataContext.WebUrl);
					break;
			}
		}
	}
}
