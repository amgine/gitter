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

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class RemoteReferencesDialog : GitDialogBase
	{
		private readonly Remote _remote;

		public RemoteReferencesDialog(Remote remote)
		{
			Verify.Argument.IsNotNull(remote, "remote");
			Verify.Argument.IsFalse(remote.IsDeleted, "remote",
				Resources.ExcObjectIsDeleted.UseAsFormat("Remote"));

			_remote = remote;

			InitializeComponent();

			Text = string.Format("{0} '{1}'", Resources.StrRemote, remote.Name);
			_lblRemoteReferences.Text = Resources.StrRemoteReferences.AddColon();

			_lstRemotes.Style = GitterApplication.DefaultStyle;
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
