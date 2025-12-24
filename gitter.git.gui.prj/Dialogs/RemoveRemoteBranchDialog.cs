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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Services;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for removing remote tracking branches.</summary>
public partial class RemoveRemoteBranchDialog : GitDialogBase
{
	private readonly RemoteBranch _branch;
	private readonly Remote? _remote;

	readonly struct DialogControls
	{
		public readonly CommandLink  _cmdRemoveLocalOnly;
		public readonly CommandLink  _cmdRemoveFromRemote;
		public readonly LabelControl _lblRemoveBranch;

		public DialogControls()
		{
			_cmdRemoveLocalOnly  = new();
			_cmdRemoveFromRemote = new();
			_lblRemoveBranch     = new();
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					padding: DpiBoundValue.Padding(new(15, 10, 15, 10)),
					rows:
					[
						LayoutConstants.LabelRowHeight,
						SizeSpec.Absolute(10),
						LayoutConstants.CommandLinkHeight,
						SizeSpec.Absolute(16),
						LayoutConstants.CommandLinkHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblRemoveBranch,     marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_cmdRemoveLocalOnly,  marginOverride: LayoutConstants.NoMargin), row: 2),
						new GridContent(new ControlContent(_cmdRemoveFromRemote, marginOverride: LayoutConstants.NoMargin), row: 4),
					]),
			};

			var tabIndex = 0;
			_lblRemoveBranch.TabIndex     = tabIndex++;
			_cmdRemoveLocalOnly.TabIndex  = tabIndex++;
			_cmdRemoveFromRemote.TabIndex = tabIndex++;

			_lblRemoveBranch.Parent     = parent;
			_cmdRemoveLocalOnly.Parent  = parent;
			_cmdRemoveFromRemote.Parent = parent;
		}

		public void Localize(RemoteBranch branch)
		{
			var remote = branch.Remote;

			_lblRemoveBranch.Text = Resources.StrsRemoveBranchFrom.UseAsFormat(branch.Name).AddColon();

			_cmdRemoveLocalOnly.Text = Resources.StrsRemoveLocalOnly;
			_cmdRemoveLocalOnly.Description = Resources.StrsRemoveLocalOnlyDescription;

			_cmdRemoveFromRemote.Text = Resources.StrsRemoveFromRemote;
			_cmdRemoveFromRemote.Description = Resources.StrsRemoveFromRemoteDescription.UseAsFormat(
				remote is not null ? remote.Name : Resources.StrRemote);
		}
	}

	private readonly DialogControls _controls;

	/// <summary>Create <see cref="RemoveRemoteBranchDialog"/>.</summary>
	/// <param name="branch"><see cref="RemoteBranch"/> to remove.</param>
	public RemoveRemoteBranchDialog(RemoteBranch branch)
	{
		Verify.Argument.IsNotNull(branch);
		Verify.Argument.IsFalse(branch.IsDeleted, nameof(branch),
			Resources.ExcObjectIsDeleted.UseAsFormat(nameof(RemoteBranch)));

		_branch = branch;
		_remote = branch.Remote;

		SuspendLayout();

		Name = nameof(RemoveRemoteBranchDialog);
		Text = Resources.StrRemoveBranch;
		Size = ScalableSize.GetValue(Dpi.Default);
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;

		_controls = new();
		_controls.Localize(branch);
		_controls.Layout(this);

		_controls._cmdRemoveLocalOnly.Click  += OnRemoveLocalOnlyClick;
		_controls._cmdRemoveFromRemote.Click += OnRemoveFromRemoteClick;

		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(350, 202);

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.Cancel;

	#region Event Handlers

	private void OnRemoveLocalOnlyClick(object? sender, EventArgs e)
	{
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				_branch.Delete(true);
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				string.Format(Resources.ErrFailedToRemoveBranch, _branch.Name),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		ClickOk();
	}

	private void OnRemoveFromRemoteClick(object? sender, EventArgs e)
	{
		try
		{
			using(this.ChangeCursor(Cursors.WaitCursor))
			{
				_branch.DeleteFromRemote();
			}
		}
		catch(GitException exc)
		{
			var branchName = _remote is not null
				? _branch.Name.Substring(_remote.Name.Length + 1)
				: _branch.Name;
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				Resources.ErrFailedToRemoveBranchFrom.UseAsFormat(branchName,
					_remote is not null ? _remote.Name : Resources.StrRemote),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		try
		{
			if(!_branch.IsDeleted)
			{
				using(this.ChangeCursor(Cursors.WaitCursor))
				{
					_branch.Delete(true);
				}
			}
		}
		catch(GitException exc)
		{
			GitterApplication.MessageBoxService.Show(
				this,
				exc.Message,
				string.Format(Resources.ErrFailedToRemoveBranch, _branch.Name),
				MessageBoxButton.Close,
				MessageBoxIcon.Error);
		}
		ClickOk();
	}

	#endregion
}
