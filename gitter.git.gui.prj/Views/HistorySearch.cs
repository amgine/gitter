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

namespace gitter.Git.Gui.Views
{
	using System;

	using gitter.Framework.Controls;

	using gitter.Git.Gui.Controls;

	class HistorySearch<T> : ListBoxSearch<T>
		where T : HistorySearchOptions
	{
		public HistorySearch(CustomListBox listBox)
			: base(listBox)
		{
		}

		protected static bool TestRevision(Revision revision, T search)
		{
			if(revision.HashString.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;
			if(!revision.IsLoaded) return false;
			var comparison = search.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
			if(revision.Subject.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Body.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Author.Name.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.Committer.Name.IndexOf(search.Text, comparison) != -1) return true;
			if(revision.TreeHashString.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;
			lock(revision.References.SyncRoot)
			{
				foreach(var reference in revision.References)
				{
					if(reference.FullName.IndexOf(search.Text, comparison) != -1) return true;
				}
			}
			return false;
		}

		protected override bool TestItem(CustomListBoxItem item, T search)
		{
			var rli = item as RevisionListItem;
			if(rli != null)
			{
				var revision = rli.DataContext;
				return TestRevision(revision, search);
			}
			return false;
		}
	}
}
