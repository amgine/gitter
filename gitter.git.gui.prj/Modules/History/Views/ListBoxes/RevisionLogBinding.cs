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

namespace gitter.Git.Gui.Controls;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

sealed class RevisionLogBinding : AsyncDataBinding<RevisionLog>
{
	private LogOptions _logOptions;

	public RevisionLogBinding(ILogSource logSource, RevisionListBox revisionListBox, LogOptions logOptions)
	{
		Verify.Argument.IsNotNull(logSource);
		Verify.Argument.IsNotNull(revisionListBox);
		Verify.Argument.IsNotNull(logOptions);

		LogSource = logSource;
		RevisionListBox = revisionListBox;
		_logOptions = logOptions;

		Progress = RevisionListBox.ProgressMonitor;
	}

	public ILogSource LogSource { get; }

	public RevisionListBox RevisionListBox { get; }

	public LogOptions LogOptions
	{
		get => _logOptions;
		set
		{
			Verify.Argument.IsNotNull(value);

			_logOptions = value;
		}
	}

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

	/// <inheritdoc/>
	protected override Task<RevisionLog> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
	{
		Verify.State.IsFalse(IsDisposed, "RevisionLogBinding is disposed.");

		RevisionListBox.Cursor = Cursors.WaitCursor;
		return LogSource.GetRevisionLogAsync(LogOptions, progress, cancellationToken);
	}

	/// <inheritdoc/>
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
			PickDefaultItemToFocus()?.FocusAndSelect();
		}
		RevisionListBox.Cursor = Cursors.Default;
	}

	/// <inheritdoc/>
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

	/// <inheritdoc/>
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
}
