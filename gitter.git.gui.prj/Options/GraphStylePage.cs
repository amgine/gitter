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
using gitter.Git.Gui.Controls;
using gitter.Git.Gui.Properties;

[ToolboxItem(false)]
sealed class GraphStylePage : PropertyPage, IExecutableDialog
{
	public static readonly new Guid Guid = new("F6B1A43A-2149-4F50-9DA8-8A59FA2CE07B");

	sealed class PreviewListBox : CustomListBox
	{
		static readonly GraphCell[][] PreviewGraph = BuidPreviewGraph();

		static GraphCell[][] BuidPreviewGraph()
		{
			var lines = new GraphCell[][]
				{
					[new()],
					[new()],
					[new(), new()],
					[new(), new()],
					[new(), new()],
					[new()],
				};

			lines[0][0].Paint(GraphElement.Dot | GraphElement.VerticalBottom, 1);
			lines[1][0].Paint(GraphElement.Dot | GraphElement.Vertical, 1);
			lines[2][0].Paint(GraphElement.Dot | GraphElement.Vertical, 1);
			lines[2][0].Paint(GraphElement.HorizontalRight, 2);
			lines[2][1].Paint(GraphElement.LeftBottomCorner, 2);
			lines[3][0].Paint(GraphElement.Vertical, 1);
			lines[3][1].Paint(GraphElement.Dot | GraphElement.Vertical, 2);
			lines[4][0].Paint(GraphElement.Dot | GraphElement.Vertical, 1);
			lines[4][0].Paint(GraphElement.HorizontalRight, 2);
			lines[4][1].Paint(GraphElement.LeftTopCorner, 2);
			lines[5][0].Paint(GraphElement.Dot | GraphElement.VerticalTop, 1);

			return lines;
		}

		sealed class Item : CustomListBoxItem, IRevisionGraphListItem
		{
			public GraphCell[]? Graph { get; set; }
		}

		public PreviewListBox(IGraphStyle graphStyle)
		{
			Columns.Add(Column = new(graphStyle) { SizeMode = ColumnSizeMode.Fill });
			HeaderStyle = HeaderStyle.Hidden;

			foreach(var entry in PreviewGraph)
			{
				Items.Add(new Item { Graph = entry });
			}
		}

		public GraphColumn Column { get; }
	}

	private readonly ICheckBoxWidget _chkRoundedCorners;
	private readonly TextBoxDecoratorWithUpDown _numLineThickness;
	private readonly ICheckBoxWidget _chkBreakLinesAroundNodes;
	private readonly ICheckBoxWidget _chkColorNodes;
	private readonly ICheckBoxWidget _chkShowColors;
	private readonly ICheckBoxWidget _chkFillBackground;
	private readonly TextBoxDecoratorWithUpDown _numNodeRadius;
	private readonly ValueSource<GraphStyleOptions> _styleOptionsSource;
	private readonly GraphStyle _style;
	private readonly PreviewListBox _preview;

	public GraphStylePage(RepositoryProvider repositoryProvider, IValueSource<GraphStyleOptions> optionsSource)
		: base(Guid)
	{
		Verify.Argument.IsNotNull(repositoryProvider);
		Verify.Argument.IsNotNull(optionsSource);

		RepositoryProvider = repositoryProvider;
		OptionsSource      = optionsSource;

		GroupSeparator group0;
		GroupSeparator group1;
		GroupSeparator group2;

		SuspendLayout();

		_chkRoundedCorners = GitterApplication.Style.CheckBoxFactory.Create();
		_chkRoundedCorners.Text = "Rounded corners";
		_chkRoundedCorners.Margin = Padding.Empty;
		_chkRoundedCorners.Parent = this;

		_chkBreakLinesAroundNodes = GitterApplication.Style.CheckBoxFactory.Create();
		_chkBreakLinesAroundNodes.Text = "Break lines around nodes";
		_chkBreakLinesAroundNodes.Margin = Padding.Empty;
		_chkBreakLinesAroundNodes.Parent = this;

		_chkColorNodes = GitterApplication.Style.CheckBoxFactory.Create();
		_chkColorNodes.Text = "Color nodes";
		_chkColorNodes.Margin = Padding.Empty;
		_chkColorNodes.Parent = this;

		_chkShowColors = GitterApplication.Style.CheckBoxFactory.Create();
		_chkShowColors.Text = Resources.StrShowColors;
		_chkShowColors.Margin = Padding.Empty;
		_chkShowColors.Parent = this;

		_chkFillBackground = GitterApplication.Style.CheckBoxFactory.Create();
		_chkFillBackground.Text = Resources.StrFillBackground;
		_chkFillBackground.Margin = Padding.Empty;
		_chkFillBackground.Parent = this;

		_style = new(_styleOptionsSource = new(new()));

		AutoScaleDimensions = new(96, 96);
		AutoScaleMode = AutoScaleMode.Dpi;
		int row = 0;
		_ = new ControlLayout(this)
		{

			Content = new Grid(
				columns:
				[
					SizeSpec.Everything(),
					SizeSpec.Absolute(12),
					SizeSpec.Absolute(210),
				],
				content:
				[
					new GridContent(new Grid(
						rows:
						[
							LayoutConstants.GroupSeparatorRowHeight,
							LayoutConstants.TextInputRowHeight,
							LayoutConstants.CheckBoxRowHeight,
							LayoutConstants.GroupSeparatorRowHeight,
							LayoutConstants.TextInputRowHeight,
							LayoutConstants.CheckBoxRowHeight,
							LayoutConstants.CheckBoxRowHeight,
							SizeSpec.Everything(),
						],
						content:
						[
							new GridContent(new ControlContent(group0 = new GroupSeparator
							{
								Text   = "Lines",
								Parent = this,
							}, marginOverride: LayoutConstants.NoMargin), row: row++),
							new GridContent(new Grid(
								columns:
								[
									SizeSpec.Absolute(80),
									SizeSpec.Absolute(60),
									SizeSpec.Everything(),
								],
								content:
								[
									new GridContent(new ControlContent(new LabelControl()
									{
										AutoSize  = false,
										Padding   = Padding.Empty,
										Text      = "Thickness:",
										Margin    = Padding.Empty,
										Parent    = this,
									}), column: 0),
									new GridContent(new ControlContent(_numLineThickness = new(new() { TextAlign = HorizontalAlignment.Right })
									{
										Minimum = 1,
										Maximum = 5,
										Parent  = this,
									}, marginOverride: LayoutConstants.TextBoxMargin), column: 1),
								]), row: row++),
							new GridContent(new WidgetContent(_chkRoundedCorners, marginOverride: LayoutConstants.NoMargin), row: row++),
							new GridContent(new ControlContent(group1 = new GroupSeparator
							{
								Text   = "Nodes",
								Parent = this,
							}, marginOverride: LayoutConstants.NoMargin), row: row++),
							new GridContent(new Grid(
								columns:
								[
									SizeSpec.Absolute(80),
									SizeSpec.Absolute(60),
									SizeSpec.Everything(),
								],
								content:
								[
									new GridContent(new ControlContent(new LabelControl()
									{
										AutoSize  = false,
										Padding   = Padding.Empty,
										Text      = "Radius:",
										Margin    = Padding.Empty,
										Parent    = this,
									}), column: 0),
									new GridContent(new ControlContent(_numNodeRadius = new(new() { TextAlign = HorizontalAlignment.Right })
									{
										Minimum = 1,
										Maximum = 5,
										Parent  = this,
									}, marginOverride: LayoutConstants.TextBoxMargin), column: 1),
								]), row: row++),
							new GridContent(new WidgetContent(_chkBreakLinesAroundNodes, marginOverride: LayoutConstants.NoMargin), row: row++),
							new GridContent(new WidgetContent(_chkColorNodes, marginOverride: LayoutConstants.NoMargin), row: row++),
						]), column: 0),
					new GridContent(new Grid(
						rows:
						[
							LayoutConstants.GroupSeparatorRowHeight,
							SizeSpec.Everything(),
							LayoutConstants.CheckBoxRowHeight,
							LayoutConstants.CheckBoxRowHeight,
						],
						content:
						[
							new GridContent(new ControlContent(group2 = new GroupSeparator
							{
								Text   = "Preview",
								Parent = this,
							}, marginOverride: LayoutConstants.NoMargin), row: 0),
							new GridContent(new ControlContent(_preview = new(_style)
							{
								Parent = this,
							}, marginOverride: LayoutConstants.NoMargin), row: 1),
							new GridContent(new WidgetContent(_chkShowColors, marginOverride: LayoutConstants.NoMargin), row: 2),
							new GridContent(new WidgetContent(_chkFillBackground, marginOverride: LayoutConstants.NoMargin), row: 3),
						]), column: 2),
				]),
		};

		group0.SendToBack();
		group1.SendToBack();

		ResumeLayout(false);
		PerformLayout();

		_preview.Column.ShowColors     = _chkShowColors.IsChecked;
		_preview.Column.FillBackground = _chkFillBackground.IsChecked;

		UpdateOptions();
		_chkColorNodes.IsCheckedChanged            += (_, _) => UpdateOptions();
		_chkRoundedCorners.IsCheckedChanged        += (_, _) => UpdateOptions();
		_chkBreakLinesAroundNodes.IsCheckedChanged += (_, _) => UpdateOptions();
		_numLineThickness.ValueChanged             += (_, _) => UpdateOptions();
		_numNodeRadius.ValueChanged                += (_, _) => UpdateOptions();
		_chkShowColors.CheckStateChanged           += (_, _) => _preview.Column.ShowColors     = _chkShowColors.IsChecked;
		_chkFillBackground.CheckStateChanged       += (_, _) => _preview.Column.FillBackground = _chkFillBackground.IsChecked;
	}

	private void UpdateOptions()
	{
		_styleOptionsSource.Value = new(
			BreakLinesWithDot: _chkBreakLinesAroundNodes.IsChecked,
			RoundedCorners:    _chkRoundedCorners.IsChecked,
			ColorNodes:        _chkColorNodes.IsChecked,
			BaseLineWidth:     _numLineThickness.Value,
			NodeRadius:        _numNodeRadius.Value);
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

		_chkRoundedCorners.IsChecked        = options.RoundedCorners;
		_numLineThickness.Value             = options.BaseLineWidth;
		_chkBreakLinesAroundNodes.IsChecked = options.BreakLinesWithDot;
		_chkColorNodes.IsChecked            = options.ColorNodes;
		_numNodeRadius.Value                = options.NodeRadius;
	}

	private GraphStyleOptions MakeSnapshot() => new(
		RoundedCorners:    _chkRoundedCorners.IsChecked,
		BaseLineWidth:     _numLineThickness.Value,
		BreakLinesWithDot: _chkBreakLinesAroundNodes.IsChecked,
		ColorNodes:        _chkColorNodes.IsChecked,
		NodeRadius:        _numNodeRadius.Value);

	public bool Execute()
	{
		var options = MakeSnapshot();
		OptionsSource.Value = options;
		var section = RepositoryProvider.ConfigSection?.GetCreateSection("GraphStyle");
		if(section is not null)
		{
			GraphStyleOptions.SaveTo(options, section);
		}
		return true;
	}
}
