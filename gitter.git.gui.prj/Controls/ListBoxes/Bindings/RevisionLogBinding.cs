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
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	sealed class RevisionLogBinding : AsyncDataBinding<RevisionLog>
	{
		#region Data

		private readonly ILogSource _logSource;
		private readonly RevisionListBox _revisionListBox;
		private LogOptions _logOptions;

		#endregion

		#region .ctor

		public RevisionLogBinding(ILogSource logSource, RevisionListBox revisionListBox, LogOptions logOptions)
		{
			Verify.Argument.IsNotNull(logSource, "logSource");
			Verify.Argument.IsNotNull(revisionListBox, "revisionListBox");
			Verify.Argument.IsNotNull(logOptions, "logOptions");

			_logSource = logSource;
			_revisionListBox = revisionListBox;
			_logOptions = logOptions;

			Progress = _revisionListBox.ProgressMonitor;
		}

		#endregion

		#region Properties

		public ILogSource LogSource
		{
			get { return _logSource; }
		}

		public RevisionListBox RevisionListBox
		{
			get { return _revisionListBox; }
		}

		public LogOptions LogOptions
		{
			get { return _logOptions; }
			set
			{
				Verify.Argument.IsNotNull(value, "value");

				_logOptions = value;
			}
		}

		#endregion

		#region Methods

		private CustomListBoxItem PickDefaultItemToFocus()
		{
			CustomListBoxItem itemToFocus = RevisionListBox.HeadItem;
			if(RevisionListBox.StagedItem != null)
			{
				itemToFocus = RevisionListBox.StagedItem;
			}
			if(RevisionListBox.UnstagedItem != null)
			{
				itemToFocus = RevisionListBox.UnstagedItem;
			}
			return itemToFocus;
		}

		protected override Task<RevisionLog> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(IsDisposed, "RevisionLogBinding is disposed.");

			RevisionListBox.Cursor = Cursors.WaitCursor;
			return LogSource.GetRevisionLogAsync(LogOptions, progress, cancellationToken);
		}

		protected override void OnFetchCompleted(RevisionLog revisionLog)
		{
			Assert.IsNotNull(revisionLog);

			if(IsDisposed || RevisionListBox.IsDisposed)
			{
				return;
			}

			bool firstTime = RevisionListBox.RevisionLog == null;
			RevisionListBox.RevisionLog = revisionLog;
			if(firstTime)
			{
				Progress = null;
				var itemToFocus = PickDefaultItemToFocus();
				if(itemToFocus != null)
				{
					itemToFocus.FocusAndSelect();
				}
			}
			RevisionListBox.Cursor = Cursors.Default;
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || RevisionListBox.IsDisposed)
			{
				return;
			}

			Progress = RevisionListBox.ProgressMonitor;
			RevisionListBox.RevisionLog = null;
			RevisionListBox.Cursor = Cursors.Default;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!RevisionListBox.IsDisposed)
				{
					RevisionListBox.Clear();
				}
			}
			base.Dispose(disposing);
		}

		#endregion
	}
}
