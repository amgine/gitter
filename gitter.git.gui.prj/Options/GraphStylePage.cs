#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Git;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Options;
using gitter.Git.Gui;

[ToolboxItem(false)]
sealed class GraphStylePage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("F6B1A43A-2149-4F50-9DA8-8A59FA2CE07B");

	private readonly CheckBox _chkRoundedCorners;
	private readonly NumericUpDown _numLineThickness;
	private readonly CheckBox _chkBreakLinesAroundNodes;
	private readonly CheckBox _chkColorNodes;
	private readonly NumericUpDown _numNodeRadius;

	public GraphStylePage(RepositoryProvider repositoryProvider, IValueSource<GraphStyleOptions> optionsSource)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(repositoryProvider);
		Verify.Argument.IsNotNull(optionsSource);

		RepositoryProvider = repositoryProvider;
		OptionsSource      = optionsSource;

		var margin = DpiBoundValue.Padding(new(3, 0, 0, 0));

		GroupSeparator group0;
		GroupSeparator group1;

		SuspendLayout();
		AutoScaleDimensions = new(96, 96);
		AutoScaleMode = AutoScaleMode.Dpi;
		int row = 0;
		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows: new[]
				{
					SizeSpec.Absolute(19),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(19),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
					SizeSpec.Everything(),
				},
				content: new[]
				{
					new GridContent(new ControlContent(group0 = new GroupSeparator
					{
						Text   = "Lines",
						Margin = Padding.Empty,
						Parent = this,
					}), row: row++),
					new GridContent(new Grid(
						columns: new[]
						{
							SizeSpec.Absolute(80),
							SizeSpec.Absolute(50),
							SizeSpec.Everything(),
						},
						content: new[]
						{
							new GridContent(new ControlContent(new Label()
							{
								AutoSize  = false,
								Padding   = Padding.Empty,
								TextAlign = ContentAlignment.MiddleLeft,
								Text      = "Thickness:",
								Margin    = Padding.Empty,
								Parent    = this,
							}), column: 0),
							new GridContent(new ControlContent(_numLineThickness = new()
							{
								Minimum = 1,
								Maximum = 5,
								Parent  = this,
							}, verticalContentAlignment: VerticalContentAlignment.Center), column: 1),
						}), row: row++),
					new GridContent(new ControlContent(_chkRoundedCorners = new()
					{
						Text   = "Rounded corners",
						Margin = Padding.Empty,
						Parent = this,
					}, marginOverride: margin), row: row++),
					new GridContent(new ControlContent(group1 = new GroupSeparator
					{
						Text   = "Nodes",
						Margin = Padding.Empty,
						Parent = this,
					}), row: row++),
					new GridContent(new Grid(
						columns: new[]
						{
							SizeSpec.Absolute(80),
							SizeSpec.Absolute(50),
							SizeSpec.Everything(),
						},
						content: new[]
						{
							new GridContent(new ControlContent(new Label()
							{
								AutoSize  = false,
								Padding   = Padding.Empty,
								TextAlign = ContentAlignment.MiddleLeft,
								Text      = "Radius:",
								Margin    = Padding.Empty,
								Parent    = this,
							}), column: 0),
							new GridContent(new ControlContent(_numNodeRadius = new()
							{
								Minimum = 1,
								Maximum = 5,
								Parent  = this,
							}, verticalContentAlignment: VerticalContentAlignment.Center), column: 1),
						}), row: row++),
					new GridContent(new ControlContent(_chkBreakLinesAroundNodes = new()
					{
						Text   = "Break lines around nodes",
						Margin = Padding.Empty,
						Parent = this,
					}, marginOverride: margin), row: row++),
					new GridContent(new ControlContent(_chkColorNodes = new()
					{
						Text   = "Color nodes",
						Margin = Padding.Empty,
						Parent = this,
					}, marginOverride: margin), row: row++),
				}),
		};

		group0.SendToBack();
		group1.SendToBack();

		ResumeLayout(false);
		PerformLayout();

		_chkRoundedCorners.CheckedChanged += OnRoundedCornersCheckedChanged;
	}

	private void OnRoundedCornersCheckedChanged(object sender, EventArgs e)
	{
		
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(448, 375));

	/// <inheritdoc/>
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		Display(OptionsSource.Value);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	private RepositoryProvider RepositoryProvider { get; }

	private IValueSource<GraphStyleOptions> OptionsSource { get; }

	private void Display(GraphStyleOptions options)
	{
		Assert.IsNotNull(options);

		_chkRoundedCorners.Checked        = options.RoundedCorners;
		_numLineThickness.Value           = options.BaseLineWidth;
		_chkBreakLinesAroundNodes.Checked = options.BreakLinesWithDot;
		_chkColorNodes.Checked            = options.ColorNodes;
		_numNodeRadius.Value              = options.NodeRadius;
	}

	private GraphStyleOptions MakeSnapshot() => new(
		RoundedCorners:    _chkRoundedCorners.Checked,
		BaseLineWidth:     (int)_numLineThickness.Value,
		BreakLinesWithDot: _chkBreakLinesAroundNodes.Checked,
		ColorNodes:        _chkColorNodes.Checked,
		NodeRadius:        (int)_numNodeRadius.Value);

	public bool Execute()
	{
		var options = MakeSnapshot();
		OptionsSource.Value = options;
		GraphStyleOptions.SaveTo(options, RepositoryProvider.ConfigSection.GetCreateSection("GraphStyle"));
		return true;
	}
}
