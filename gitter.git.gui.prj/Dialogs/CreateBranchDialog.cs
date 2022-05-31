﻿#region Copyright Notice
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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

using gitter.Framework;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using gitter.Git.AccessLayer;

using gitter.Git.Gui.Controllers;
using gitter.Git.Gui.Interfaces;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Dialog for creating <see cref="Branch"/> object.</summary>
[ToolboxItem(false)]
public partial class CreateBranchDialog : GitDialogBase, IAsyncExecutableDialog, ICreateBranchView
{
	private Repository _repository;
	private ICreateBranchController _controller;
	private bool _branchNameEdited;

	/// <summary>Create <see cref="CreateBranchDialog"/>.</summary>
	/// <param name="repository"><see cref="Repository"/> to create <see cref="Branch"/> in.</param>
	/// <exception cref="ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
	public CreateBranchDialog(Repository repository)
	{
		Verify.Argument.IsNotNull(repository);

		_repository = repository;

		InitializeComponent();
		Localize();

		var inputs = new IUserInputSource[]
		{
			BranchName       = new TextBoxInputSource(_txtName),
			StartingRevision = new ControlInputSource(_txtRevision),
			Checkout         = new CheckBoxInputSource(_chkCheckoutAfterCreation),
			Orphan           = new CheckBoxInputSource(_chkOrphan),
			CreateReflog     = new CheckBoxInputSource(_chkCreateReflog),
			TrackingMode     = new RadioButtonGroupInputSource<BranchTrackingMode>(
				new[]
				{
					Tuple.Create(_trackingDefault,    BranchTrackingMode.Default),
					Tuple.Create(_trackingTrack,      BranchTrackingMode.Tracking),
					Tuple.Create(_trackingDoNotTrack, BranchTrackingMode.NotTracking),
				}),
		};

		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		SetupReferenceNameInputBox(_txtName, ReferenceType.LocalBranch);

		ToolTipService.Register(_chkCheckoutAfterCreation, Resources.TipCheckoutAfterCreation);
		ToolTipService.Register(_chkOrphan, Resources.TipOrphan);
		ToolTipService.Register(_chkCreateReflog, Resources.TipReflog);
		ToolTipService.Register(_trackingTrack, Resources.TipTrack);

		_txtRevision.References.LoadData(
			_repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_txtRevision.References.Items[0].IsExpanded = true;

		GitterApplication.FontManager.InputFont.Apply(_txtName, _txtRevision);
		GlobalBehavior.SetupAutoCompleteSource(_txtRevision, _repository, ReferenceType.Branch);

		_controller = new CreateBranchController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 200));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public IUserInputSource<string> StartingRevision { get; }

	public IUserInputSource<string> BranchName { get; }

	public IUserInputSource<BranchTrackingMode> TrackingMode { get; }

	public IUserInputSource<bool> Checkout { get; }

	public IUserInputSource<bool> Orphan { get; }

	public IUserInputSource<bool> CreateReflog { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	private void Localize()
	{
		Text = Resources.StrCreateBranch;

		_lblName.Text = Resources.StrName.AddColon();
		_lblRevision.Text = Resources.StrRevision.AddColon();

		_grpOptions.Text = Resources.StrOptions;
		_chkCheckoutAfterCreation.Text = Resources.StrCheckoutAfterCreation;
		if(GitFeatures.CheckoutOrphan.IsAvailableFor(_repository))
		{
			_chkOrphan.Text = Resources.StrlMakeOrphanBranch;
		}
		else
		{
			_chkOrphan.Text = Resources.StrlMakeOrphanBranch + " " +
				Resources.StrfVersionRequired.UseAsFormat(GitFeatures.CheckoutOrphan.RequiredVersion).SurroundWithBraces();
		}
		_chkCreateReflog.Text = Resources.StrCreateBranchReflog;

		_grpTracking.Text = Resources.StrsTrackingMode;
		_trackingDefault.Text = Resources.StrDefault;
		_trackingDoNotTrack.Text = Resources.StrlDoNotTrack;
		_trackingTrack.Text = Resources.StrTrack;
	}

	private void OnBranchNameChanged(object sender, EventArgs e)
	{
		_branchNameEdited = !string.IsNullOrEmpty(_txtName.Text);
	}

	private void OnRevisionChanged(object sender, EventArgs e)
	{
		if(!_branchNameEdited)
		{
			var branchName = _txtRevision.Text.Trim();
			var branch     = _repository.Refs.Remotes.TryGetItem(branchName);
			if(branch is not null)
			{
				_txtName.Text = branch.Name.Substring(branch.Name.LastIndexOf('/') + 1);
				_branchNameEdited = false;
			}
		}
	}

	private void OnCheckoutAfterCreationCheckedChanged(object sender, EventArgs e)
	{
		_chkOrphan.Enabled = _chkCheckoutAfterCreation.Checked
			&& GitFeatures.CheckoutOrphan.IsAvailableFor(_repository);
	}

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryCreateBranchAsync();
}
