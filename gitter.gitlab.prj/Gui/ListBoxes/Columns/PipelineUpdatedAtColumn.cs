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

namespace gitter.GitLab.Gui
{
	using System;
	using System.Drawing;
	using gitter.Framework.Controls;

	using Resources = gitter.GitLab.Properties.Resources;

	sealed class PipelineUpdatedAtColumn : DateColumn
	{
		public PipelineUpdatedAtColumn()
			: base((int)ColumnId.UpdatedAt, Resources.StrUpdatedAt, true)
		{
			Width = 100;
			DateFormat = Framework.DateFormat.Relative;
		}

		public override string IdentificationString => "UpdatedAt";

		protected override Comparison<CustomListBoxItem> SortComparison => PipelineListItem.CompareByUpdatedAt;

		private static bool TryGetContent(CustomListBoxItem item, out DateTimeOffset value)
		{
			if(item is PipelineListItem pipeline)
			{
				var v = pipeline.DataContext.UpdatedAt;
				if(v.HasValue)
				{
					value = v.Value;
					return true;
				}
			}
			value = default;
			return false;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			if(TryGetContent(measureEventArgs.Item, out var value))
			{
				return OnMeasureSubItem(measureEventArgs, value);
			}
			return base.OnMeasureSubItem(measureEventArgs);
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(TryGetContent(paintEventArgs.Item, out var value))
			{
				OnPaintSubItem(paintEventArgs, value);
				return;
			}
			base.OnPaintSubItem(paintEventArgs);
		}
	}
}
