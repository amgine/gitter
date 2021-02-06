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
	using System.Windows.Forms;

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="CustomListBox"/> for displaying <see cref="Repository.Stash"/> contents.</summary>
	public sealed class StashListBox : CustomListBox
	{
		#region Columns

		private readonly CustomListBoxColumn _colHash;
		private readonly CustomListBoxColumn _colSubject;
		private readonly CustomListBoxColumn _colCommitDate;
		private readonly CustomListBoxColumn _colCommitter;
		private readonly CustomListBoxColumn _colCommitterEmail;
		private readonly CustomListBoxColumn _colAuthorDate;
		private readonly CustomListBoxColumn _colAuthor;
		private readonly CustomListBoxColumn _colAuthorEmail;

		#endregion

		#region Data

		private Repository _repository;

		#endregion

		/// <summary>Create <see cref="StashListBox"/>.</summary>
		public StashListBox()
		{
			Columns.AddRange(new[]
				{
					_colHash			= new HashColumn(),
					_colCommitDate		= new CommitDateColumn(),
					_colSubject			= new SubjectColumn(false)
					{
						AlignToGraph = false,
						ShowLocalBranches = false,
						ShowRemoteBranches = false,
						ShowTags = false,
						ShowStash = false,
					},
					_colCommitter		= new CommitterColumn(),
					_colCommitterEmail	= new CommitterEmailColumn(),
					_colAuthorDate		= new AuthorDateColumn(),
					_colAuthor			= new AuthorColumn(),
					_colAuthorEmail		= new AuthorEmailColumn(),
				});

			Text = Resources.StrsNothingStashed;
		}

		public void LoadData(Repository repository)
		{
			if(_repository != null)
				DetachFromRepository();
			_repository = repository;
			if(_repository != null)
				AttachToRepository();
		}

		private void AttachToRepository()
		{
			BeginUpdate();
			Items.Clear();
			lock(_repository.Stash.SyncRoot)
			{
				foreach(var ss in _repository.Stash)
				{
					Items.Add(new StashedStateListItem(ss));
				}
			}
			EndUpdate();
			_repository.Stash.StashedStateCreated += OnStashCreated;
		}

		private void OnStashCreated(object sender, StashedStateEventArgs e)
		{
			Items.AddSafe(new StashedStateListItem(e.Object));
		}

		private void DetachFromRepository()
		{
			_repository.Stash.StashedStateCreated -= OnStashCreated;
			Items.Clear();
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_repository != null)
				{
					DetachFromRepository();
					_repository = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
