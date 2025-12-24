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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Git.Gui.Controls;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
public partial class ResolveCheckoutDialog : GitDialogBase
{
	readonly struct DialogControls
	{
		public readonly ICheckBoxWidget _chkDontShowAgain;
		public readonly CommandLink _btnCheckoutCommit;
		public readonly CommandLink _btnCheckoutBranch;
		public readonly ReferencesListBox _references;
		public readonly LabelControl _lblSelectOther;

		public DialogControls(Many<Branch> branches, IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_chkDontShowAgain  = style.CheckBoxFactory.Create();
			_btnCheckoutBranch = new();
			_btnCheckoutCommit = new();
			_references = new()
			{
				Style          = style,
				HeaderStyle    = gitter.Framework.Controls.HeaderStyle.Hidden,
				ItemActivation = gitter.Framework.Controls.ItemActivation.SingleClick,
				ShowTreeLines  = false
			};
			_lblSelectOther = new();
			_references.BeginUpdate();
			_references.Style = GitterApplication.Style;
			_references.Items.Clear();
			var first = default(Branch);
			foreach(var branch in branches)
			{
				if(branch.IsCurrent) continue;
				first ??= branch;
				_references.Items.Add(new BranchListItem(branch));
			}
			_references.EndUpdate();
			if(_references.Items.Count <= 1)
			{
				_references.Visible = false;
				_lblSelectOther.Visible = false;
			}
			ScalableSize = new SizeImpl(_references.Visible);
		}

		public void Localize()
		{
			_btnCheckoutCommit.Description = "This will detach HEAD and make you unable to create new commits, merge, revert, etc.";
			_btnCheckoutCommit.Text = "Checkout commit";
			_btnCheckoutBranch.Description = "This will bring working tree to the same state, but HEAD will point to selected branch.";
			_btnCheckoutBranch.Text = "Checkout \'%BRANCH NAME%\'";
			_lblSelectOther.Text = "Select other branch:";
			_chkDontShowAgain.Text = "Don\'t ask again, always checkout commits";
		}

		sealed class SizeImpl(bool showRefs) : IDpiBoundValue<Size>
		{
			public Size GetValue(Dpi dpi)
			{
				var conv = DpiConverter.FromDefaultTo(dpi);
				var h = 0;
				h += conv.ConvertY(16) * 4;
				h += conv.ConvertY(66) * 2;
				if(showRefs)
				{
					h += LayoutConstants.LabelRowHeight.GetSize(int.MaxValue, dpi);
					h += LayoutConstants.LabelRowSpacing.GetSize(int.MaxValue, dpi);
					h += conv.ConvertY(66);
				}
				h += LayoutConstants.CheckBoxRowHeight.GetSize(int.MaxValue, dpi);
				return new(conv.ConvertX(350), h);
			}
		}

		public IDpiBoundValue<Size> ScalableSize { get; }

		public void Layout(Control parent)
		{
			if(_references.Visible)
			{
				_ = new ControlLayout(parent)
				{
					Content = new Grid(
						padding: DpiBoundValue.Padding(new(16)),
						rows:
						[
							SizeSpec.Absolute(66),
							SizeSpec.Absolute(16),
							SizeSpec.Absolute(66),
							SizeSpec.Absolute(16),
							LayoutConstants.LabelRowHeight,
							LayoutConstants.LabelRowSpacing,
							SizeSpec.Absolute(66),
							LayoutConstants.CheckBoxRowHeight,
						],
						content:
						[
							new GridContent(new ControlContent(_btnCheckoutCommit, marginOverride: LayoutConstants.NoMargin), row: 0),
							new GridContent(new ControlContent(_btnCheckoutBranch, marginOverride: LayoutConstants.NoMargin), row: 2),
							new GridContent(new ControlContent(_lblSelectOther,    marginOverride: LayoutConstants.NoMargin), row: 4),
							new GridContent(new ControlContent(_references,        marginOverride: LayoutConstants.NoMargin), row: 6),
							new GridContent(new WidgetContent (_chkDontShowAgain,  marginOverride: LayoutConstants.NoMargin), row: 7),
						]),
				};
			}
			else
			{
				_ = new ControlLayout(parent)
				{
					Content = new Grid(
						padding: DpiBoundValue.Padding(new(16)),
						rows:
						[
							SizeSpec.Absolute(66),
							SizeSpec.Absolute(16),
							SizeSpec.Absolute(66),
							SizeSpec.Absolute(16),
							LayoutConstants.CheckBoxRowHeight,
						],
						content:
						[
							new GridContent(new ControlContent(_btnCheckoutCommit, marginOverride: LayoutConstants.NoMargin), row: 0),
							new GridContent(new ControlContent(_btnCheckoutBranch, marginOverride: LayoutConstants.NoMargin), row: 2),
							new GridContent(new WidgetContent (_chkDontShowAgain,  marginOverride: LayoutConstants.NoMargin), row: 4),
						]),
				};
			}

			var tabIndex = 0;
			_chkDontShowAgain.TabIndex = tabIndex++;
			_btnCheckoutCommit.TabIndex = tabIndex++;
			_btnCheckoutBranch.TabIndex = tabIndex++;
			if(_references.Visible)
			{
				_references.TabIndex     = tabIndex++;
				_lblSelectOther.TabIndex = tabIndex++;
			}

			_chkDontShowAgain.Parent  = parent;
			_btnCheckoutCommit.Parent = parent;
			_btnCheckoutBranch.Parent = parent;
			if(_references.Visible)
			{
				_references.Parent     = parent;
				_lblSelectOther.Parent = parent;
			}
		}
	}

	private readonly DialogControls _controls;

	public ResolveCheckoutDialog(Many<Branch> branches)
	{
		_controls = new(branches, GitterApplication.Style);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode = AutoScaleMode.Dpi;
		Name = nameof(ResolveCheckoutDialog);
		Text = Resources.StrCheckout;
		Size = ScalableSize.GetValue(Dpi.Default);

		_controls.Localize();
		_controls.Layout(this);

		_controls._btnCheckoutCommit.Click += _btnCheckoutCommit_Click;
		_controls._btnCheckoutBranch.Click += _btnCheckoutBranch_Click;
		_controls._references.ItemActivated += OnItemActivated;

		if(_controls._references.Items.Count != 0)
		{
			SelectedBranch = ((BranchListItem)_controls._references.Items[0]).DataContext;
			UpdateButton();
		}

		ResumeLayout(performLayout: false);
		PerformLayout();
	}

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.Cancel;

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize => _controls.ScalableSize;

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	public bool CheckoutCommit { get; private set; } = true;

	public Branch? SelectedBranch { get; private set; }

	private void UpdateButton()
	{
		if(SelectedBranch is null) return;
		_controls._btnCheckoutBranch.Text = string.Format("{0} '{1}'", Resources.StrCheckout, SelectedBranch.Name);
	}

	private void OnItemActivated(object? sender, ItemEventArgs e)
	{
		var b = ((BranchListItem)e.Item).DataContext;
		SelectedBranch = b;
		UpdateButton();
	}

	private void _btnCheckoutCommit_Click(object? sender, EventArgs e)
	{
		CheckoutCommit = true;
		GlobalBehavior.AskOnCommitCheckouts = !_controls._chkDontShowAgain.IsChecked;
		ClickOk();
	}

	private void _btnCheckoutBranch_Click(object? sender, EventArgs e)
	{
		CheckoutCommit = false;
		GlobalBehavior.AskOnCommitCheckouts = !_controls._chkDontShowAgain.IsChecked;
		ClickOk();
	}
}
