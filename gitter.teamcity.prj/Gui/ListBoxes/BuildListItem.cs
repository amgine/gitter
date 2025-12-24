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

namespace gitter.TeamCity.Gui;

using System;
using System.Globalization;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

sealed class BuildListItem(Build buildType)
	: CustomListBoxItem<Build>(buildType)
{
	protected override void OnListBoxAttached(CustomListBox listBox)
	{
		base.OnListBoxAttached(listBox);

		DataContext.PropertyChanged += OnBuildPropertyChanged;
	}

	protected override void OnListBoxDetached(CustomListBox listBox)
	{
		DataContext.PropertyChanged -= OnBuildPropertyChanged;

		base.OnListBoxDetached(listBox);
	}

	private IImageProvider? StatusImage
		=> DataContext.Status switch
		{
			BuildStatus.Success => Icons.StatusSuccess,
			BuildStatus.Failure => Icons.StatusFailure,
			BuildStatus.Error   => Icons.StatusError,
			_ => null,
		};

	private void OnBuildPropertyChanged(object? sender, TeamCityObjectPropertyChangedEventArgs e)
	{
		if(e.Property == Build.IdProperty)
		{
			InvalidateSubItemSafe((int)ColumnId.Id);
		}
		else if(e.Property == Build.Properties.Status)
		{
			InvalidateSubItemSafe((int)ColumnId.Status);
		}
		else if(e.Property == Build.Properties.BuildType)
		{
			InvalidateSubItemSafe((int)ColumnId.BuildType);
		}
		else if(e.Property == Build.Properties.StartDate)
		{
			InvalidateSubItemSafe((int)ColumnId.StartDate);
		}
		else if(e.Property == Build.Properties.BranchName)
		{
			InvalidateSubItemSafe((int)ColumnId.BranchName);
		}
		else if(e.Property == Build.Properties.Number)
		{
			InvalidateSubItemSafe((int)ColumnId.Number);
		}
		else if(e.Property == Build.WebUrlProperty)
		{
			InvalidateSubItemSafe((int)ColumnId.WebUrl);
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Id:
				return measureEventArgs.MeasureText(DataContext.Id);
			case ColumnId.Status:
				return measureEventArgs.MeasureImageAndText(StatusImage?.GetImage(measureEventArgs.Dpi.X * 16 / 96), DataContext.Status.ToString());
			case ColumnId.Number:
				return measureEventArgs.MeasureText(DataContext.Number);
			case ColumnId.BranchName:
				return measureEventArgs.MeasureText(DataContext.BranchName);
			case ColumnId.StartDate:
				return DateColumn.OnMeasureSubItem(measureEventArgs, DataContext.StartDate);
			case ColumnId.WebUrl:
				return measureEventArgs.MeasureText(DataContext.WebUrl);
			default:
				return Size.Empty;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Id:
				paintEventArgs.PaintText(DataContext.Id.ToString(CultureInfo.InvariantCulture));
				break;
			case ColumnId.Status:
				paintEventArgs.PaintImageAndText(StatusImage?.GetImage(paintEventArgs.Dpi.X * 16 / 96), DataContext.Status.ToString());
				break;
			case ColumnId.Number:
				paintEventArgs.PaintText(DataContext.Number);
				break;
			case ColumnId.BranchName:
				paintEventArgs.PaintText(DataContext.BranchName);
				break;
			case ColumnId.StartDate:
				DateColumn.OnPaintSubItem(paintEventArgs, DataContext.StartDate);
				break;
			case ColumnId.WebUrl:
				paintEventArgs.PaintText(DataContext.WebUrl);
				break;
		}
	}
}
