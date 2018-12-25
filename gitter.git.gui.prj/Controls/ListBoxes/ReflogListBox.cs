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

	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	/// <summary><see cref="CustomListBox"/> for displaying <see cref="ReflogRecordListItem"/>.</summary>
	public sealed class ReflogListBox : CustomListBox
	{
		#region Data

		private readonly CustomListBoxColumn _colHash;
		private readonly CustomListBoxColumn _colTreeHash;
		private readonly CustomListBoxColumn _colMessage;
		private readonly CustomListBoxColumn _colSubject;
		private readonly CustomListBoxColumn _colCommitDate;
		private readonly CustomListBoxColumn _colCommitter;
		private readonly CustomListBoxColumn _colCommitterEmail;
		private readonly CustomListBoxColumn _colAuthorDate;
		private readonly CustomListBoxColumn _colAuthor;
		private readonly CustomListBoxColumn _colAuthorEmail;

		private Reflog _reflog;

		#endregion

		public ReflogListBox()
		{
			Columns.AddRange(new[]
				{
					_colHash			= new HashColumn()				{ IsVisible = true,  Abbreviate = true, Width = 58 },
					_colTreeHash		= new TreeHashColumn()			{ IsVisible = false, Abbreviate = true, Width = 58 },
					_colCommitDate		= new CommitDateColumn()		{ IsVisible = false },
					_colMessage			= new MessageColumn()			{ IsVisible = true },
					_colSubject			= new SubjectColumn()			{ IsVisible = false },
					_colCommitter		= new CommitterColumn()			{ IsVisible = false },
					_colCommitterEmail	= new CommitterEmailColumn()	{ IsVisible = false },
					_colAuthorDate		= new AuthorDateColumn()		{ IsVisible = false },
					_colAuthor			= new AuthorColumn()			{ IsVisible = false },
					_colAuthorEmail		= new AuthorEmailColumn()		{ IsVisible = false },
				});
		}

		public Reflog Reflog
		{
			get { return _reflog; }
		}

		public void Load(Reflog reflog)
		{
			if(_reflog != reflog)
			{
				if(_reflog != null)
				{
					DetachFromReflog();
				}
				_reflog = reflog;
				if(_reflog != null)
				{
					AttachToReflog();
				}
			}
		}

		private void AttachToReflog()
		{
			lock(_reflog)
			{
				foreach(var record in _reflog)
				{
					Items.Add(new ReflogRecordListItem(record));
				}
				_reflog.RecordAdded += OnRecordAdded;
			}
		}

		private void DetachFromReflog()
		{
			_reflog.RecordAdded -= OnRecordAdded;
			Items.Clear();
		}

		private void OnRecordAdded(object sender, ReflogRecordEventArgs e)
		{
			var item = new ReflogRecordListItem(e.Object);
			Items.InsertSafe(e.Object.Index, item);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(_reflog != null)
				{
					DetachFromReflog();
					_reflog = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
