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

		protected static bool TestHash(string hash, T search)
		{
			return hash is not null && hash.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase);
		}

		protected static bool TestRevision(Revision revision, T search)
		{
			Assert.IsNotNull(revision);
			Assert.IsNotNull(search);

			if(TestHash(revision.HashString, search)) return true;
			if(revision.IsLoaded)
			{
				if(TestString(revision.Subject, search)) return true;
				if(TestString(revision.Body, search)) return true;
				if(TestString(revision.Author.Name, search)) return true;
				if(TestString(revision.Committer.Name, search)) return true;
				if(TestHash(revision.TreeHashString, search)) return true;
			}
			lock(revision.References.SyncRoot)
			{
				foreach(var reference in revision.References)
				{
					if(TestString(reference.FullName, search)) return true;
				}
			}
			return false;
		}

		protected override bool TestItem(CustomListBoxItem item, T search)
			=> item is RevisionListItem rli && TestRevision(rli.DataContext, search);
	}
}
