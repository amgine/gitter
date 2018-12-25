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
	using System.Collections.Generic;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Controls;

	using Resources = gitter.Properties.Resources;

	public sealed class RepositoryRootItem : CustomListBoxItem
	{
		#region Static

		private static readonly Image ImgRepository = CachedResources.Bitmaps["ImgRepository"];

		#endregion

		#region Data

		private readonly IWorkingEnvironment _environment;
		private string _repository;

		#endregion

		#region .ctor

		public RepositoryRootItem(IWorkingEnvironment environment, string repository)
		{
			Verify.Argument.IsNotNull(environment, nameof(environment));

			_environment = environment;
			_repository = repository;
			Expand();
		}

		#endregion

		public string RepositoryDisplayName
		{
			get { return _repository; }
			set
			{
				_repository = value;
				Invalidate();
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					if(_repository == null)
					{
						return measureEventArgs.MeasureText("<no repository>");
					}
					else
					{
						return measureEventArgs.MeasureImageAndText(ImgRepository, _repository);
					}
				default:
					return Size.Empty;
			}
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					if(_repository == null)
						paintEventArgs.PaintText("<no repository>");
					else
						paintEventArgs.PaintImageAndText(ImgRepository, _repository);
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			if(_repository != null)
			{
				var menu = new ContextMenuStrip();
				var item = new ToolStripMenuItem(
					Resources.StrAddService.AddEllipsis(), null,
					(s, e) => ShowAddServiceDialog());
				menu.Items.Add(item);
				Utility.MarkDropDownForAutoDispose(menu);
				return menu;
			}
			return base.GetContextMenu(requestEventArgs);
		}

		private void ShowAddServiceDialog()
		{
			using(var d = new AddServiceDialog(_environment))
			{
				d.Run(ListBox);
			}
		}
	}
}
