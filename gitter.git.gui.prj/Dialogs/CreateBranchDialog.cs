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

namespace gitter.Git.Gui.Dialogs;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

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
	private readonly DialogControls _controls;
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

		Name = nameof(CreateBranchDialog);
		Text = Resources.StrCreateBranch;

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Size                = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize(_repository);
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		var inputs = new IUserInputSource[]
		{
			BranchName       = new TextBoxInputSource(_controls._txtName),
			StartingRevision = new ControlInputSource(_controls._txtRevision),
			Checkout         = new CheckBoxWidgetInputSource(_controls._chkCheckoutAfterCreation),
			Orphan           = new CheckBoxWidgetInputSource(_controls._chkOrphan),
			CreateReflog     = new CheckBoxWidgetInputSource(_controls._chkCreateReflog),
			TrackingMode     = new RadioButtonWidgetGroupInputSource<BranchTrackingMode>(
				[
					Tuple.Create(_controls._trackingDefault,    BranchTrackingMode.Default),
					Tuple.Create(_controls._trackingTrack,      BranchTrackingMode.Tracking),
					Tuple.Create(_controls._trackingDoNotTrack, BranchTrackingMode.NotTracking),
				]),
		};

		ErrorNotifier = new UserInputErrorNotifier(NotificationService, inputs);

		_controls._txtName.TextChanged                       += OnBranchNameChanged;
		_controls._txtRevision.TextChanged                   += OnRevisionChanged;
		_controls._chkCheckoutAfterCreation.IsCheckedChanged += OnCheckoutAfterCreationCheckedChanged;

		SetupReferenceNameInputBox(_controls._txtName, ReferenceType.LocalBranch);

		ToolTipService.Register(_controls._chkCheckoutAfterCreation.Control, Resources.TipCheckoutAfterCreation);
		ToolTipService.Register(_controls._chkOrphan.Control, Resources.TipOrphan);
		ToolTipService.Register(_controls._chkCreateReflog.Control, Resources.TipReflog);
		ToolTipService.Register(_controls._trackingTrack.Control, Resources.TipTrack);

		_controls._txtRevision.References.LoadData(
			_repository,
			ReferenceType.Reference,
			GlobalBehavior.GroupReferences,
			GlobalBehavior.GroupRemoteBranches);
		_controls._txtRevision.References.Items[0].IsExpanded = true;

		GlobalBehavior.SetupAutoCompleteSource((TextBox)_controls._txtRevision.Decorated, _repository, ReferenceType.Branch);

		_controller = new CreateBranchController(repository) { View = this };
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(400, 196));

	/// <inheritdoc/>
	protected override string ActionVerb => Resources.StrCreate;

	public IUserInputSource<string> StartingRevision { get; }

	public IUserInputSource<string?> BranchName { get; }

	public IUserInputSource<BranchTrackingMode> TrackingMode { get; }

	public IUserInputSource<bool> Checkout { get; }

	public IUserInputSource<bool> Orphan { get; }

	public IUserInputSource<bool> CreateReflog { get; }

	public IUserInputErrorNotifier ErrorNotifier { get; }

	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BeginInvoke(_controls._txtName.Focus);
	}

	private void OnBranchNameChanged(object? sender, EventArgs e)
	{
		_branchNameEdited = !string.IsNullOrEmpty(_controls._txtName.Text);
	}

	private void OnRevisionChanged(object? sender, EventArgs e)
	{
		if(!_branchNameEdited)
		{
			var branchName = _controls._txtRevision.Text?.Trim() ?? "";
			var branch     = _repository.Refs.Remotes.TryGetItem(branchName);
			if(branch is not null)
			{
				_controls._txtName.Text = branch.Name.Substring(branch.Name.LastIndexOf('/') + 1);
				_branchNameEdited = false;
			}
		}
	}

	private void OnCheckoutAfterCreationCheckedChanged(object? sender, EventArgs e)
	{
		_controls._chkOrphan.Enabled = _controls._chkCheckoutAfterCreation.IsChecked
			&& GitFeatures.CheckoutOrphan.IsAvailableFor(_repository);
	}

	/// <inheritdoc/>
	public Task<bool> ExecuteAsync() => _controller.TryCreateBranchAsync();
}
