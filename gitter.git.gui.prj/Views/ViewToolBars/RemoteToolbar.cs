namespace gitter.Git.Gui.Views
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using gitter.Git.Gui.Dialogs;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	sealed class RemoteToolbar : ToolStrip
	{
		private readonly RemoteView _remoteView;

		public RemoteToolbar(RemoteView remoteView)
		{
			Verify.Argument.IsNotNull(remoteView, "remoteView");

			_remoteView = remoteView;

			Items.Add(new ToolStripButton(Resources.StrRefresh, CachedResources.Bitmaps["ImgRefresh"],
				(sender, e) => _remoteView.RefreshContent())
			{
				DisplayStyle = ToolStripItemDisplayStyle.Image,
			});
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripButton(Resources.StrFetch, CachedResources.Bitmaps["ImgFetch"], OnFetchClick)
				{
					ToolTipText = Resources.TipFetch,
				});
			Items.Add(new ToolStripButton(Resources.StrPull, CachedResources.Bitmaps["ImgPull"], OnPullClick)
				{
					ToolTipText = Resources.TipPull,
				});
			Items.Add(new ToolStripButton(Resources.StrPush, CachedResources.Bitmaps["ImgPush"], OnPushClick)
				{
					ToolTipText = Resources.TipPush,
				});
			Items.Add(new ToolStripSeparator());
			Items.Add(new ToolStripButton(Resources.StrPrune, CachedResources.Bitmaps["ImgClean"], OnPruneClick));
		}

		private void OnFetchClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				try
				{
					remote.FetchAsync().Invoke<ProgressForm>(_remoteView);
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						_remoteView,
						exc.Message,
						string.Format(Resources.ErrFailedToFetchFrom, remote.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private void OnPullClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				try
				{
					remote.PullAsync().Invoke<ProgressForm>(_remoteView);
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						_remoteView,
						exc.Message,
						string.Format(Resources.ErrFailedToPullFrom, remote.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}

		private void OnPushClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				using(var dlg = new PushDialog(_remoteView.Repository)
					{
						Remote = remote,
					})
				{
					dlg.Run(_remoteView);
				}
			}
		}

		private void OnPruneClick(object sender, EventArgs e)
		{
			var remote = _remoteView.Remote;
			if(remote != null && !remote.IsDeleted)
			{
				try
				{
					remote.PruneAsync().Invoke<ProgressForm>(_remoteView);
				}
				catch(GitException exc)
				{
					GitterApplication.MessageBoxService.Show(
						_remoteView,
						exc.Message,
						string.Format(Resources.ErrFailedToPrune, remote.Name),
						MessageBoxButton.Close,
						MessageBoxIcon.Error);
				}
			}
		}
	}
}
