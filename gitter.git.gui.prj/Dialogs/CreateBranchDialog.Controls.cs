#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using Resources = gitter.Git.Gui.Properties.Resources;

partial class CreateBranchDialog
{
	readonly struct DialogControls
	{
		private readonly LabelControl _lblName;
		public  readonly TextBox _txtName;
		private readonly TextBoxDecorator _txtNameDecorator;
		private readonly LabelControl _lblRevision;
		public  readonly gitter.Git.Gui.Controls.RevisionPicker _txtRevision;
		private readonly GroupSeparator _grpOptions;
		public  readonly ICheckBoxWidget _chkCheckoutAfterCreation;
		public  readonly ICheckBoxWidget _chkOrphan;
		public  readonly ICheckBoxWidget _chkCreateReflog;
		private readonly GroupSeparator _grpTracking;
		public  readonly IRadioButtonWidget _trackingDoNotTrack;
		public  readonly IRadioButtonWidget _trackingTrack;
		public  readonly IRadioButtonWidget _trackingDefault;

		public DialogControls(IGitterStyle? style = default)
		{
			style ??= GitterApplication.Style;

			_lblName = new();
			_txtName          = new TextBox();
			_txtNameDecorator = new(_txtName);
			_lblRevision = new();
			_txtRevision = new gitter.Git.Gui.Controls.RevisionPicker();

			var cff = style.CheckBoxFactory;
			var rbf = style.RadioButtonFactory;
			_grpOptions               = new GroupSeparator();
			_chkCheckoutAfterCreation = cff.Create();
			_chkOrphan                = cff.Create();
			_chkCreateReflog          = cff.Create();
			_grpTracking              = new GroupSeparator();
			_trackingDoNotTrack       = rbf.Create();
			_trackingTrack            = rbf.Create();
			_trackingDefault          = rbf.Create();

			_trackingDefault.IsChecked = true;

			GitterApplication.FontManager.InputFont.Apply(_txtName, _txtRevision);
		}

		public void Layout(Control parent)
		{
			Panel pnlTrackingMode;

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.TextInputRowHeight,
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.CheckBoxRowHeight,
						LayoutConstants.GroupSeparatorRowHeight,
						LayoutConstants.RadioButtonRowHeight,
						SizeSpec.Everything(),
					],
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new ControlContent(_lblName, marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 0, column: 0),
						new GridContent(new ControlContent(_txtNameDecorator, marginOverride: LayoutConstants.TextBoxMargin), row: 0, column: 1),
						new GridContent(new ControlContent(_lblRevision, marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 1, column: 0),
						new GridContent(new ControlContent(_txtRevision, marginOverride: LayoutConstants.TextBoxMargin), row: 1, column: 1),

						new GridContent(new ControlContent(_grpOptions, marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 2, column: 0, columnSpan: 2),

						new GridContent(new WidgetContent(_chkCheckoutAfterCreation,
							marginOverride: LayoutConstants.GroupPadding), row: 3, column: 0, columnSpan: 2),
						new GridContent(new WidgetContent(_chkOrphan,
							marginOverride: LayoutConstants.GroupPadding), row: 4, column: 0, columnSpan: 2),
						new GridContent(new WidgetContent(_chkCreateReflog,
							marginOverride: LayoutConstants.GroupPadding), row: 5, column: 0, columnSpan: 2),

						new GridContent(new ControlContent(_grpTracking, marginOverride: DpiBoundValue.Padding(new(0, 6, 0, 6))), row: 6, column: 0, columnSpan: 2),

						new GridContent(new ControlContent(pnlTrackingMode = new()
						{
							Parent = parent,
						}, marginOverride: LayoutConstants.GroupPadding), row: 7, column: 0, columnSpan: 2),
					]),
			};

			pnlTrackingMode.SuspendLayout();

			_ = new ControlLayout(pnlTrackingMode)
			{
				Content = new Grid(
					columns:
					[
						SizeSpec.Absolute(94),
						SizeSpec.Absolute(120),
						SizeSpec.Absolute(94),
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new WidgetContent(_trackingDefault,
							marginOverride: LayoutConstants.NoMargin), column: 0),
						new GridContent(new WidgetContent(_trackingDoNotTrack,
							marginOverride: LayoutConstants.NoMargin), column: 1),
						new GridContent(new WidgetContent(_trackingTrack,
							marginOverride: LayoutConstants.NoMargin), column: 2),
					]),
			};

			var tabIndex = 0;
			_lblName.TabIndex                  = tabIndex++;
			_txtNameDecorator.TabIndex         = tabIndex++;
			_txtName.TabIndex                  = tabIndex++;
			_lblRevision.TabIndex              = tabIndex++;
			_txtRevision.TabIndex              = tabIndex++;
			_grpOptions.TabIndex               = tabIndex++;
			_chkCheckoutAfterCreation.TabIndex = tabIndex++;
			_chkOrphan.TabIndex                = tabIndex++;
			_chkCreateReflog.TabIndex          = tabIndex++;
			_grpTracking.TabIndex              = tabIndex++;
			pnlTrackingMode.TabIndex           = tabIndex++;
			_trackingDefault.TabIndex          = tabIndex++;
			_trackingDoNotTrack.TabIndex       = tabIndex++;
			_trackingTrack.TabIndex            = tabIndex++;

			_lblName.Parent                  = parent;
			_txtNameDecorator.Parent         = parent;
			_lblRevision.Parent              = parent;
			_txtRevision.Parent              = parent;
			_grpOptions.Parent               = parent;
			_chkCheckoutAfterCreation.Parent = parent;
			_chkOrphan.Parent                = parent;
			_chkCreateReflog.Parent          = parent;
			_grpTracking.Parent              = parent;
			_trackingDefault.Parent          = pnlTrackingMode;
			_trackingDoNotTrack.Parent       = pnlTrackingMode;
			_trackingTrack.Parent            = pnlTrackingMode;

			pnlTrackingMode.ResumeLayout();
			pnlTrackingMode.PerformLayout();
		}

		public void Localize(Repository repository)
		{
			_lblName.Text = Resources.StrName.AddColon();
			_lblRevision.Text = Resources.StrRevision.AddColon();

			_grpOptions.Text = Resources.StrOptions;
			_chkCheckoutAfterCreation.Text = Resources.StrCheckoutAfterCreation;
			if(GitFeatures.CheckoutOrphan.IsAvailableFor(repository))
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch;
			}
			else
			{
				_chkOrphan.Text = Resources.StrlMakeOrphanBranch + " " +
					Resources.StrfVersionRequired.UseAsFormat(GitFeatures.CheckoutOrphan.RequiredVersion).SurroundWithBraces();
			}
			_chkCreateReflog.Text = Resources.StrCreateBranchReflog;

			_grpTracking.Text        = Resources.StrsTrackingMode;
			_trackingDefault.Text    = Resources.StrDefault;
			_trackingDoNotTrack.Text = Resources.StrlDoNotTrack;
			_trackingTrack.Text      = Resources.StrTrack;
		}
	}
}
