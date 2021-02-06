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

	/// <summary>"Commit Date" column.</summary>
	public sealed class CommitDateColumn : DateColumn
	{
		public CommitDateColumn()
			: base((int)ColumnId.CommitDate, Resources.StrCommitDate, true)
		{
			Width = 106;
		}

		public override string IdentificationString => "CommitDate";

		private static bool TryGetCommitDate(CustomListBoxItem item, out DateTimeOffset commitDate)
		{
			if(item is IRevisionPointerListItem revisionItem)
			{
				var revision = revisionItem.RevisionPointer.Dereference();
				if(revision != null)
				{
					commitDate = revision.CommitDate;
					return true;
				}
			}
			commitDate = default;
			return false;
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			if(TryGetCommitDate(measureEventArgs.Item, out var commitDate))
			{
				return OnMeasureSubItem(measureEventArgs, commitDate);
			}
			return base.OnMeasureSubItem(measureEventArgs);
		}

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			if(TryGetCommitDate(paintEventArgs.Item, out var commitDate))
			{
				OnPaintSubItem(paintEventArgs, commitDate);
			}
		}
	}
}
