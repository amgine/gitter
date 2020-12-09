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

namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Drawing;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary>"Author Date" column.</summary>
	public sealed class AuthorDateColumn : DateColumn
	{
		public AuthorDateColumn()
			: base((int)ColumnId.AuthorDate, Resources.StrAuthorDate, false)
		{
			Width = 106;
		}

		public override string IdentificationString => "AuthorDate";

		private static bool TryGetAuthorDate(CustomListBoxItem item, out DateTimeOffset authorDate)
		{
			if(item is IRevisionPointerListItem revisionItem)
			{
				var revision = revisionItem.RevisionPointer.Dereference();
				if(revision != null)
				{
					authorDate = revision.AuthorDate;
					return true;
				}
			}
			authorDate = default;
			return false;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			if(TryGetAuthorDate(measureEventArgs.Item, out var authorDate))
			{
				return OnMeasureSubItem(measureEventArgs, authorDate);
			}
			return base.OnMeasureSubItem(measureEventArgs);
		}

		protected override void OnPainSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(TryGetAuthorDate(paintEventArgs.Item, out var authorDate))
			{
				OnPaintSubItem(paintEventArgs, authorDate);
			}
		}
	}
}
