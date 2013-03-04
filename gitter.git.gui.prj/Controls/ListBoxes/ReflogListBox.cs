namespace gitter.Git.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Git.Gui.Dialogs;

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
					_colHash			= new HashColumn()				{ IsVisible = true, Abbreviate = true, Width = 58 },
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
