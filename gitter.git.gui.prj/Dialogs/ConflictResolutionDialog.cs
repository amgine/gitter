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

using Resources = gitter.Git.Gui.Properties.Resources;

internal partial class ConflictResolutionDialog : GitDialogBase
{
	readonly struct DialogControls
	{
		public  readonly CommandLink  _btnResolution1;
		public  readonly CommandLink  _btnResolution2;
		private readonly LabelControl _label1;
		public  readonly LabelControl _lblFileName;
		private readonly LabelControl _label3;
		private readonly LabelControl _label4;
		public  readonly Label _lblOursStatus;
		public  readonly Label _lblTheirsStatus;

		public DialogControls(IGitterStyle? style)
		{
			style ??= GitterApplication.Style;

			_btnResolution2 = new();
			_btnResolution1 = new();
			_label1 = new();
			_lblFileName = new();
			_label3 = new();
			_label4 = new();
			_lblOursStatus = new();
			_lblTheirsStatus = new();

			_lblOursStatus.BorderStyle   = BorderStyle.FixedSingle;
			_lblTheirsStatus.BorderStyle = BorderStyle.FixedSingle;
			_lblOursStatus.TextAlign     = ContentAlignment.MiddleCenter;
			_lblTheirsStatus.TextAlign   = ContentAlignment.MiddleCenter;
			_lblOursStatus.ForeColor     = Color.Black;
			_lblTheirsStatus.ForeColor   = Color.Black;
		}

		public void Layout(Control parent)
		{
			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					rows:
					[
						/* 0 */ LayoutConstants.LabelRowHeight,
						/* 1 */ LayoutConstants.LabelRowHeight,
						/* 2 */ LayoutConstants.LabelRowSpacing,
						/* 3 */ LayoutConstants.LabelRowHeight,
						/* 4 */ SizeSpec.Absolute(7),
						/* 5 */ SizeSpec.Absolute(26),
						/* 6 */ SizeSpec.Absolute(7),
						/* 7 */ SizeSpec.Absolute(26),
					],
					content:
					[
						new GridContent(new ControlContent(_label1, marginOverride: LayoutConstants.NoMargin), row: 0),
						new GridContent(new ControlContent(_lblFileName, marginOverride: LayoutConstants.NoMargin), row: 1),
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(75),
								SizeSpec.Absolute(75),
								SizeSpec.Absolute(8),
								SizeSpec.Absolute(75),
								SizeSpec.Absolute(75),
							],
							content:
							[
								new GridContent(new ControlContent(_label3, marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new ControlContent(_lblOursStatus, marginOverride: LayoutConstants.NoMargin), column: 1),
								new GridContent(new ControlContent(_label4, marginOverride: LayoutConstants.NoMargin), column: 3),
								new GridContent(new ControlContent(_lblTheirsStatus, marginOverride: LayoutConstants.NoMargin), column: 4),
							]), row: 3),
						new GridContent(new ControlContent(_btnResolution1, marginOverride: LayoutConstants.NoMargin), row: 5),
						new GridContent(new ControlContent(_btnResolution2, marginOverride: LayoutConstants.NoMargin), row: 7),
					]),
			};

			var tabIndex = 0;
			_btnResolution1.TabIndex = tabIndex++;
			_btnResolution2.TabIndex = tabIndex++;
			_label1.TabIndex = tabIndex++;
			_lblFileName.TabIndex = tabIndex++;
			_label3.TabIndex = tabIndex++;
			_label4.TabIndex = tabIndex++;
			_lblOursStatus.TabIndex = tabIndex++;
			_lblTheirsStatus.TabIndex = tabIndex++;

			_btnResolution1.Parent = parent;
			_btnResolution2.Parent = parent;
			_label1.Parent = parent;
			_lblFileName.Parent = parent;
			_label3.Parent = parent;
			_label4.Parent = parent;
			_lblOursStatus.Parent = parent;
			_lblTheirsStatus.Parent = parent;
		}

		public void Localize()
		{
			_btnResolution2.Text = "Delete file";
			_btnResolution1.Text = "Keep modified file";
			_label1.Text = "File:";
			_label3.Text = "Ours status:";
			_label4.Text = "Theirs status:";
		}
	}

	private readonly DialogControls _controls;
	private readonly ConflictResolution _resolution1;
	private readonly ConflictResolution _resolution2;

	private static string StatusToString(FileStatus status)
		=> status switch
		{
			FileStatus.Added    => Resources.StrlAdded,
			FileStatus.Removed  => Resources.StrlDeleted,
			FileStatus.Modified => Resources.StrlModified,
			_ => throw new ArgumentException($"Unknown status: {status}", nameof(status)),
		};

	private static Color StatusToColor(FileStatus status)
		=> status switch
		{
			FileStatus.Added    => Color.Green,
			FileStatus.Removed  => Color.Red,
			FileStatus.Modified => Color.Yellow,
			_ => throw new ArgumentException($"Unknown status: {status}", nameof(status)),
		};

	private static string ConflictResolutionToString(ConflictResolution conflictResolution)
		=> conflictResolution switch
		{
			ConflictResolution.KeepModifiedFile => Resources.StrsKeepModifiedFile,
			ConflictResolution.DeleteFile       => Resources.StrsDeleteFile,
			ConflictResolution.UseOurs          => Resources.StrsUseOursVersion,
			ConflictResolution.UseTheirs        => Resources.StrsUseTheirsVersion,
			_ => throw new ArgumentException($"Unknown resolution: {conflictResolution}", nameof(conflictResolution)),
		};

	public ConflictResolutionDialog(string fileName, FileStatus oursStatus, FileStatus theirsStatus,
		ConflictResolution resolution1, ConflictResolution resolution2)
	{
		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		Name = nameof(ConflictResolutionDialog);
		Text = Resources.StrConflictResolution;
		Size = ScalableSize.GetValue(Dpi.Default);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		_controls._btnResolution1.Click += OnResolution1Click;
		_controls._btnResolution2.Click += OnResolution2Click;


		_controls._lblFileName.Text = fileName;

		_controls._lblOursStatus.Text = StatusToString(oursStatus);
		_controls._lblTheirsStatus.Text = StatusToString(theirsStatus);

		_controls._lblOursStatus.BackColor = StatusToColor(oursStatus);
		_controls._lblTheirsStatus.BackColor = StatusToColor(theirsStatus);

		_resolution1 = resolution1;
		_resolution2 = resolution2;

		_controls._btnResolution1.Text = ConflictResolutionToString(resolution1);
		_controls._btnResolution2.Text = ConflictResolutionToString(resolution2);
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(350, 125));

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	/// <inheritdoc/>
	public override DialogButtons OptimalButtons => DialogButtons.Cancel;

	/// <inheritdoc/>
	public ConflictResolution ConflictResolution { get; private set; }

	private void OnResolution1Click(object? sender, EventArgs e)
	{
		ConflictResolution = _resolution1;
		ClickOk();
	}

	private void OnResolution2Click(object? sender, EventArgs e)
	{
		ConflictResolution = _resolution2;
		ClickOk();
	}
}
