namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RemoteReferencesDialog : GitDialogBase
	{
		private readonly Remote _remote;

		public RemoteReferencesDialog(Remote remote)
		{
			if(remote == null) throw new ArgumentNullException("remote");
			if(remote.IsDeleted) throw new ArgumentException(string.Format(Resources.ExcObjectIsDeleted, "remote"), "remote");
			_remote = remote;

			InitializeComponent();

			Text = string.Format("{0} '{1}'", Resources.StrRemote, remote.Name);
			_lblRemoteReferences.Text = Resources.StrRemoteReferences.AddColon();

			_lstRemotes.Load(_remote);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrClose; }
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Ok; }
		}

		protected override void OnShown()
		{
			base.OnShown();
			var action = AsyncAction.Create(
				_remote,
				(remote, monitor) =>
				{
					_lstRemotes.Load(remote);
					_lstRemotes.FetchData();
				},
				Resources.StrFetchingDataFrom.UseAsFormat(_remote.Name),
				string.Empty);
			action.BeginInvoke(this, _lstRemotes.ProgressMonitor, OnFethCompleted, action);
		}

		private void OnFethCompleted(IAsyncResult ar)
		{
			var action = (AsyncAction<Remote>)ar.AsyncState;
			try
			{
				action.EndInvoke(ar);
			}
			catch(GitException exc)
			{
				if(!_lstRemotes.IsDisposed)
				{
					try
					{
						_lstRemotes.BeginInvoke(new Action<string>(
							msg =>
							{
								if(!_lstRemotes.IsDisposed)
								{
									_lstRemotes.Text = msg;
								}
							}), exc.Message);
					}
					catch
					{
					}
				}
			}
		}
	}
}
