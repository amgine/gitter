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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Branches"/> &amp; <see cref="Repository.Tags"/>.</summary>
	public sealed class ReferencesListBox : CustomListBox
	{
		#region Data

		private readonly CustomListBoxColumn _colName;
		private readonly CustomListBoxColumn _colHash;
		private readonly CustomListBoxColumn _colTreeHash;

		private ReferenceTreeBinding _refBinding;

		#endregion

		public ReferencesListBox()
		{
			Columns.AddRange(new[]
				{
					_colName = new NameColumn(),
					_colHash = new HashColumn(),
					_colTreeHash = new TreeHashColumn(),
				});
			ShowTreeLines = true;
		}

		public void EnableCheckboxes()
		{
			foreach(var item in Items)
			{
				EnableCheckboxes(item, true);
			}
			ShowCheckBoxes = true;
		}

		public void DisableCheckboxes()
		{
			foreach(var item in Items)
			{
				EnableCheckboxes(item, false);
			}
			ShowCheckBoxes = false;
		}

		private static void EnableCheckboxes(CustomListBoxItem item, bool state)
		{
			if(state && item is IRevisionPointerListItem)
			{
				item.CheckedState = CheckedState.Unchecked;
			}
			else
			{
				item.CheckedState = CheckedState.Unavailable;
			}
			foreach(var i in item.Items)
			{
				EnableCheckboxes(i, state);
			}
		}

		public Repository Repository
		{
			get
			{
				if(_refBinding != null)
				{
					return _refBinding.Repository;
				}
				return null;
			}
		}

		public void LoadData(Repository repository)
		{
			LoadData(repository, ReferenceType.Reference, true, true, null);
		}

		public void LoadData(Repository repository, ReferenceType referenceTypes, bool groupItems, bool groupRemoteBranches)
		{
			LoadData(repository, referenceTypes, groupItems, groupRemoteBranches, null);
		}

		public void LoadData(Repository repository, ReferenceType referenceTypes, bool groupItems, bool groupRemoteBranches, Predicate<IRevisionPointer> predicate)
		{
			if(_refBinding != null)
			{
				_refBinding.Dispose();
				_refBinding = null;
			}

			if(repository == null) return;

			BeginUpdate();

			_refBinding = new ReferenceTreeBinding(Items, repository, groupItems, groupRemoteBranches,
				predicate, referenceTypes);

			if(!groupRemoteBranches) ShowTreeLines = false;

			EndUpdate();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_refBinding != null)
				{
					_refBinding.Dispose();
					_refBinding = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
