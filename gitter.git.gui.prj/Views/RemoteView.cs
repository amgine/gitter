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

namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	using gitter.Framework;
	
	using gitter.Git.Gui.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	partial class RemoteView : GitViewBase
	{
		private sealed class RemoteReferencesDataSource : AsyncDataBinding<RemoteReferencesCollection>
		{
			#region Data

			private readonly Remote _remote;
			private readonly RemoteReferencesListBox _listBox;
			private readonly RemoteReferencesCollection _remoteReferences;

			#endregion

			#region .ctor

			public RemoteReferencesDataSource(Remote remote, RemoteReferencesListBox listBox)
			{
				_remote           = remote;
				_listBox          = listBox;
				_remoteReferences = remote.GetReferences();

				listBox.RemoteReferences = _remoteReferences;
				Progress = listBox.ProgressMonitor;
			}

			#endregion

			private RemoteReferencesListBox ListBox
			{
				get { return _listBox; }
			}

			protected override Task<RemoteReferencesCollection> FetchDataAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
			{
				ListBox.Text = string.Empty;
				ListBox.Cursor = Cursors.WaitCursor;
				return _remoteReferences.RefreshAsync(progress, cancellationToken)
					.ContinueWith(
					t =>
					{
						TaskUtility.PropagateFaultedStates(t);
						return _remoteReferences;
					},
					cancellationToken,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default);
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

		#region Data

		private RemoteToolbar _toolbar;
		private Remote _remote;
		private RemoteReferencesDataSource _dataSource;

		#endregion

		#region .ctor

		public RemoteView(GuiProvider gui)
			: base(Guids.RemoteViewGuid, gui)
		{
			InitializeComponent();

			Text = Resources.StrRemote;

			AddTopToolStrip(_toolbar = new RemoteToolbar(this));
		}

		#endregion

		#region Properties

		public override Image Image
		{
			get { return CachedResources.Bitmaps["ImgRemote"]; }
		}

		public override bool IsDocument
		{
			get { return true; }
		}

		public Remote Remote
		{
			get { return _remote; }
			private set
			{
				if(IsDisposed) throw new ObjectDisposedException(GetType().Name);

				if(_remote != value)
				{
					if(_remote != null)
					{
						DetachRemote(_remote);
					}
					_remote = value;
					if(_remote != null)
					{
						AttachRemote(_remote);
					}

					UpdateText();
					if(value != null)
					{
						DataSource = new RemoteReferencesDataSource(value, _lstRemoteReferences);
					}
					else
					{
						DataSource = null;
					}
				}
			}
		}

		private RemoteReferencesDataSource DataSource
		{
			get { return _dataSource; }
			set
			{
				if(_dataSource != value)
				{
					if(_dataSource != null)
					{
						_dataSource.Dispose();
					}
					_dataSource = value;
					if(_dataSource != null)
					{
						_dataSource.ReloadData();
					}
				}
			}
		}

		#endregion

		#region Methods

		protected override void AttachViewModel(object viewModel)
		{
			base.AttachViewModel(viewModel);

			var vm = viewModel as RemoteViewModel;
			if(vm != null)
			{
				Remote = vm.Remote;
			}
		}

		protected override void DetachViewModel(object viewModel)
		{
			base.DetachViewModel(viewModel);

			var vm = viewModel as RemoteViewModel;
			if(vm != null)
			{
				Remote = null;
			}
		}

		public override void RefreshContent()
		{
			if(DataSource != null)
			{
				DataSource.ReloadData();
			}
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
				if(Remote != null)
				{
					Text = Remote.Name;
				}
				else
				{
					Text = Resources.StrRemote;
				}
			}
		}

		private void OnRemoteRenamed(object sender, NameChangeEventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(UpdateText));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					UpdateText();
				}
			}
		}

		private void OnRemoteDeleted(object sender, EventArgs e)
		{
			if(!IsDisposed)
			{
				if(InvokeRequired)
				{
					try
					{
						BeginInvoke(new MethodInvoker(Close));
					}
					catch(ObjectDisposedException)
					{
					}
				}
				else
				{
					Close();
				}
			}
		}

		#endregion
	}
}
