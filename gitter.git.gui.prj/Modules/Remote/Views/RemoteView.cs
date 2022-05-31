#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git.Gui.Views;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;

using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

partial class RemoteView : GitViewBase, ISearchableView<RemoteSearchOptions>
{
	private sealed class RemoteReferencesDataSource : AsyncDataBinding<RemoteReferencesCollection>
	{
		#region Data

		private readonly Remote _remote;
		private readonly RemoteReferencesCollection _remoteReferences;

		#endregion

		#region .ctor

		public RemoteReferencesDataSource(Remote remote, RemoteReferencesListBox listBox)
		{
			_remote           = remote;
			ListBox           = listBox;
			_remoteReferences = remote.GetReferences();

			listBox.RemoteReferences = _remoteReferences;
			Progress = listBox.ProgressMonitor;
		}

		#endregion

		private RemoteReferencesListBox ListBox { get; }

		protected override async Task<RemoteReferencesCollection> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			ListBox.Text = string.Empty;
			ListBox.Cursor = Cursors.WaitCursor;
			await _remoteReferences.RefreshAsync(progress, cancellationToken);
			return _remoteReferences;
		}

		protected override void OnFetchCompleted(RemoteReferencesCollection data)
		{
			if(IsDisposed || ListBox.IsDisposed)
			{
				return;
			}

			ListBox.ProgressMonitor.Report(OperationProgress.Completed);
			ListBox.Cursor = Cursors.Default;
		}

		protected override void OnFetchFailed(Exception exception)
		{
			if(IsDisposed || ListBox.IsDisposed)
			{
				return;
			}

			ListBox.ProgressMonitor.Report(OperationProgress.Completed);
			ListBox.Text = exception.Message;
			ListBox.RemoteReferences = null;
			ListBox.Cursor = Cursors.Default;
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(!ListBox.IsDisposed)
				{
					ListBox.RemoteReferences = null;
				}
			}
			base.Dispose(disposing);
		}
	}

	private readonly RemoteReferencesListBox _lstRemoteReferences;
	private RemoteToolbar _toolbar;
	private readonly ISearchToolBarController _searchToolbar;
	private Remote _remote;
	private RemoteReferencesDataSource _dataSource;

	public RemoteView(GuiProvider gui, IFactory<RemoteReferencesListBox> remoteReferencesListBoxfactory)
		: base(Guids.RemoteViewGuid, gui)
	{
		Verify.Argument.IsNotNull(remoteReferencesListBoxfactory);

		SuspendLayout();
		_lstRemoteReferences = remoteReferencesListBoxfactory.Create();
		_lstRemoteReferences.Name          = nameof(RemoteView);
		_lstRemoteReferences.Dock          = DockStyle.Fill;
		_lstRemoteReferences.BorderStyle   = BorderStyle.None;
		_lstRemoteReferences.Name          = nameof(_lstRemoteReferences);
		_lstRemoteReferences.ShowTreeLines = true;
		_lstRemoteReferences.TabIndex      = 0;
		_lstRemoteReferences.Parent        = this;
		ResumeLayout(false);
		PerformLayout();

		Text = Resources.StrRemote;
		Search = new RemoteSearch(_lstRemoteReferences);

		_searchToolbar = CreateSearchToolbarController<RemoteView, RemoteSearchToolBar, RemoteSearchOptions>(this);

		_lstRemoteReferences.PreviewKeyDown += OnKeyDown;

		AddTopToolStrip(_toolbar = new RemoteToolbar(this));
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DataSource = null;
		}
		base.Dispose(disposing);
	}

	public override IImageProvider ImageProvider => Icons.Remote;

	public override bool IsDocument => true;

	public Remote Remote
	{
		get => _remote;
		private set
		{
			if(IsDisposed) throw new ObjectDisposedException(GetType().Name);

			if(_remote != value)
			{
				if(_remote is not null)
				{
					DetachRemote(_remote);
				}
				_remote = value;
				if(_remote is not null)
				{
					AttachRemote(_remote);
				}

				UpdateText();
				DataSource = value is not null
					? new RemoteReferencesDataSource(value, _lstRemoteReferences)
					: null;
			}
		}
	}

	private RemoteReferencesDataSource DataSource
	{
		get => _dataSource;
		set
		{
			if(_dataSource != value)
			{
				if(_dataSource is not null)
				{
					_dataSource.Dispose();
				}
				_dataSource = value;
				if(_dataSource is not null)
				{
					_dataSource.ReloadData();
				}
			}
		}
	}

	public ISearch<RemoteSearchOptions> Search { get; }

	public bool SearchToolBarVisible
	{
		get => _searchToolbar.IsVisible;
		set => _searchToolbar.IsVisible = value;
	}

	protected override void AttachViewModel(object viewModel)
	{
		base.AttachViewModel(viewModel);

		if(viewModel is RemoteViewModel vm)
		{
			Remote = vm.Remote;
		}
	}

	protected override void DetachViewModel(object viewModel)
	{
		base.DetachViewModel(viewModel);

		if(viewModel is RemoteViewModel)
		{
			Remote = null;
		}
	}

	public override void RefreshContent()
	{
		DataSource?.ReloadData();
	}

	private void DetachRemote(Remote remote)
	{
		remote.Deleted -= OnRemoteDeleted;
		remote.Renamed -= OnRemoteRenamed;
	}

	private void AttachRemote(Remote remote)
	{
		remote.Deleted += OnRemoteDeleted;
		remote.Renamed += OnRemoteRenamed;
	}

	private void UpdateText()
	{
		if(!IsDisposed)
		{
			Text = Remote is not null
				? Remote.Name
				: Resources.StrRemote;
		}
	}

	private void OnRemoteRenamed(object sender, NameChangeEventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(UpdateText));
			}
			catch
			{
			}
		}
		else
		{
			UpdateText();
		}
	}

	private void OnRemoteDeleted(object sender, EventArgs e)
	{
		if(IsDisposed) return;
		if(InvokeRequired)
		{
			try
			{
				BeginInvoke(new MethodInvoker(Close));
			}
			catch
			{
			}
		}
		else
		{
			Close();
		}
	}

	protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
	{
		OnKeyDown(this, e);
		base.OnPreviewKeyDown(e);
	}

	private void OnKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		Assert.IsNotNull(e);

		switch(e.KeyCode)
		{
			case Keys.F when e.Modifiers == Keys.Control:
				_searchToolbar.Show();
				break;
			case Keys.F5:
				RefreshContent();
				e.IsInputKey = true;
				break;
		}
	}
}
