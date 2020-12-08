#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2020  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework;
	using gitter.Framework.Controls;

	abstract class RepositoryExplorerItemBase : CustomListBoxItem
	{
		private readonly string _text;
		private Bitmap _image;

		protected RepositoryExplorerItemBase(IWorkingEnvironment env, GitLabGuiProvider guiProvider, Bitmap image, string text)
		{
			WorkingEnvironment = env;
			GuiProvider = guiProvider;
			_image = image;
			_text = text;
		}

		protected IWorkingEnvironment WorkingEnvironment { get; }

		protected GitLabGuiProvider GuiProvider { get; }

		//protected void ShowView(Guid guid)
		//{
		//	if(WorkingEnvironment.ViewDockService.ShowView(guid) is GitLabViewBase view)
		//	{
		//		view.ServiceContext = GuiProvider.ServiceContext;
		//	}
		//}

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
}
