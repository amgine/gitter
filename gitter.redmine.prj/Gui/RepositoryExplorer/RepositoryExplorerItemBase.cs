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

namespace gitter.Redmine.Gui;

using System;
using System.Drawing;

using gitter.Framework;
using gitter.Framework.Controls;

abstract class RepositoryExplorerItemBase : CustomListBoxItem
{
	private readonly string _text;
	private IImageProvider _image;

	protected RepositoryExplorerItemBase(IWorkingEnvironment env, RedmineGuiProvider guiProvider, IImageProvider image, string text)
	{
		WorkingEnvironment = env;
		GuiProvider = guiProvider;
		_image = image;
		_text = text;
	}

	protected IWorkingEnvironment WorkingEnvironment { get; private set; }

	protected RedmineGuiProvider GuiProvider { get; private set; }

	protected RedmineServiceContext ServiceContext
		=> GuiProvider.ServiceContext;

	protected void ShowView(Guid guid)
	{
		var view = WorkingEnvironment.ViewDockService.ShowView(guid) as RedmineViewBase;
		if(view is not null)
		{
			view.ServiceContext = ServiceContext;
		}
	}

	protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
	{
		switch((ColumnId)paintEventArgs.SubItemId)
		{
			case ColumnId.Name:
				paintEventArgs.PaintImageAndText(_image, _text);
				break;
		}
	}

	protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
	{
		switch((ColumnId)measureEventArgs.SubItemId)
		{
			case ColumnId.Name:
				return measureEventArgs.MeasureImageAndText(_image, _text);
			default:
				return Size.Empty;
		}
	}
}
