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
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

abstract class RepositoryExplorerItemBase : CustomListBoxItem
{
	private string _text;
	private IImageProvider _icon;

	protected RepositoryExplorerItemBase(IWorkingEnvironment env, TeamCityGuiProvider guiProvider, IImageProvider icon, string text)
	{
		WorkingEnvironment = env;
		GuiProvider = guiProvider;
		_icon = icon;
		_text = text;
	}

	protected IWorkingEnvironment WorkingEnvironment { get; private set; }

	protected TeamCityGuiProvider GuiProvider { get; private set; }

	protected TeamCityServiceContext ServiceContext
		=> GuiProvider.ServiceContext;

	protected IImageProvider Icon
	{
		get => _icon;
		set
		{
			if(_icon == value) return;

			_icon = value;
			InvalidateSafe();
		}
	}

	protected string Text
	{
		get => _text;
		set
		{
			if(_text == value) return;

			_text = value;
			InvalidateSafe();
		}
	}

	protected void ShowView(Guid guid)
	{
		if(WorkingEnvironment.ViewDockService.ShowView(guid) is TeamCityViewBase view)
		{
			view.ServiceContext = ServiceContext;
		}
	}

	/// <inheritdoc/>
	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		Assert.IsNotNull(paintEventArgs);

		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(_icon?.GetImage(paintEventArgs.Dpi.X * 16 / 96), _text);
				break;
		}
	}

	/// <inheritdoc/>
	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		Assert.IsNotNull(measureEventArgs);

		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(_icon?.GetImage(measureEventArgs.Dpi.X * 16 / 96), _text);
			default:
				return Size.Empty;
		}
	}
}
