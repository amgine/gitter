#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

	class ReferencesSearch : ListBoxTreeSearch<ReferencesSearchOptions>
	{
		public ReferencesSearch(CustomListBox listBox)
			: base(listBox)
		{
		}

		protected static bool TestBranch(Branch branch, ReferencesSearchOptions search)
		{
			Assert.IsNotNull(branch);
			Assert.IsNotNull(search);

			if(TestString(branch.Name, search)) return true;
			if(branch.Revision.HashString.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;

			return false;
		}

		protected static bool TestRemoteBranch(RemoteBranch branch, ReferencesSearchOptions search)
		{
			Assert.IsNotNull(branch);
			Assert.IsNotNull(search);

			if(TestString(branch.Name, search)) return true;
			if(branch.Revision.HashString.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;

			return false;
		}

		protected static bool TestTag(Tag tag, ReferencesSearchOptions search)
		{
			Assert.IsNotNull(tag);
			Assert.IsNotNull(search);

			if(TestString(tag.Name, search)) return true;
			if(tag.Revision.HashString.StartsWith(search.Text, StringComparison.OrdinalIgnoreCase)) return true;

			return false;
		}

		protected override bool TestItem(CustomListBoxItem item, ReferencesSearchOptions search)
			=> item switch
			{
				BranchListItem       bli  => TestBranch       (bli.DataContext,  search),
				RemoteBranchListItem rbli => TestRemoteBranch (rbli.DataContext, search),
				TagListItem          tli  => TestTag          (tli.DataContext,  search),
				_ => false,
			};
	}
}
