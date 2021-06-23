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

namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	public abstract class RepositoryExplorerItemBase : CustomListBoxItem
	{
		private readonly string _imageName;
		private readonly string _text;
		private Repository _repository;

		protected RepositoryExplorerItemBase(string imageName, string text)
		{
			_imageName = imageName;
			_text      = text;
		}

		private Image GetImage(Dpi dpi)
			=> CachedResources.ScaledBitmaps[_imageName, DpiConverter.FromDefaultTo(dpi).ConvertX(16)];

		/// <inheritdoc/>
		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			Assert.IsNotNull(paintEventArgs);

			switch((ColumnId)paintEventArgs.SubItemId)
			{
				case ColumnId.Name:
					paintEventArgs.PaintImageAndText(GetImage(paintEventArgs.Dpi), _text);
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
					return measureEventArgs.MeasureImageAndText(GetImage(measureEventArgs.Dpi), _text);
				default:
					return Size.Empty;
			}
		}

		public Repository Repository
		{
			get => _repository;
			set
			{
				if(_repository != value)
				{
					if(_repository is not null)
					{
						DetachFromRepository();
					}
					_repository = value;
					if(_repository is not null)
					{
						AttachToRepository();
					}
				}
			}
		}

		protected virtual void DetachFromRepository()
		{
		}

		protected virtual void AttachToRepository()
		{
		}
	}
}
