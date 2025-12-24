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

namespace gitter.Framework.Options;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
public partial class AppearancePage : PropertyPage, IExecutableDialog
{
	private readonly Panel _pnlRestartRequiredWarning;
	private readonly IRadioButtonWidget _radGdi;
	private readonly IRadioButtonWidget _radGdiPlus;
	private IGitterStyle _selectedStyle;

	public AppearancePage()
		: base(PropertyPageFactory.AppearanceGroupGuid)
	{
		SuspendLayout();

		Panel pnlTextRenderers;
		Panel pnlStyles;

		_selectedStyle = GitterApplication.StyleOnNextStartup;

		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows:
				[
					LayoutConstants.GroupSeparatorRowHeight,
					LayoutConstants.RadioButtonRowHeight,
					LayoutConstants.RadioButtonRowHeight,
					LayoutConstants.GroupSeparatorRowHeight,
					SizeSpec.Everything(),
					SizeSpec.Absolute(23),
				],
				content:
				[
					new GridContent(new ControlContent(new GroupSeparator()
					{
						Margin = Padding.Empty,
						Text   = "Preferred text renderer",
						Parent = this,
					}), row: 0),
					new GridContent(new ControlContent(pnlTextRenderers = new()
					{
						Margin = Padding.Empty,
						Parent = this,
					}), row: 1, rowSpan: 2),
					new GridContent(new ControlContent(new GroupSeparator()
					{
						Margin = Padding.Empty,
						Text   = "Application theme",
						Parent = this,
					}), row: 3),
					new GridContent(new ControlContent(pnlStyles = new()
					{
						Margin = Padding.Empty,
						Parent = this,
					}), row: 4),
					new GridContent(new ControlContent(_pnlRestartRequiredWarning = new()
					{
						Margin = Padding.Empty,
						Parent = this,
					}), row: 5),
				]),
		};

		_radGdi = GitterApplication.Style.RadioButtonFactory.Create();
		_radGdi.Text      = "GDI";
		_radGdi.IsChecked = true;
		_radGdi.Parent    = pnlTextRenderers;
		_radGdiPlus = GitterApplication.Style.RadioButtonFactory.Create();
		_radGdiPlus.Enabled = false;
		_radGdiPlus.Text    = "GDI+";
		_radGdiPlus.Parent  = pnlTextRenderers;

		_ = new ControlLayout(pnlTextRenderers)
		{
			Content = new Grid(
				padding: LayoutConstants.GroupPadding,
				rows:
				[
					LayoutConstants.RadioButtonRowHeight,
					LayoutConstants.RadioButtonRowHeight,
					SizeSpec.Everything(),
				],
				content:
				[
					new GridContent(new WidgetContent(_radGdi,     marginOverride: LayoutConstants.NoMargin), row: 0),
					new GridContent(new WidgetContent(_radGdiPlus, marginOverride: LayoutConstants.NoMargin), row: 1),
				]),
		};

		var styleRows    = new List<ISizeSpec>();
		var styleContent = new List<GridContent>();
		var row = 0;
		foreach(var style in GitterApplication.Styles)
		{
			styleRows.Add(LayoutConstants.RadioButtonRowHeight);
			var button = GitterApplication.Style.RadioButtonFactory.Create();
			button.Text      = style.DisplayName;
			button.Tag       = style;
			button.IsChecked = style == SelectedStyle;
			button.Parent    = pnlStyles;
			button.IsCheckedChanged += OnThemeRadioButtonCheckedChanged;
			styleContent.Add(new GridContent(new WidgetContent(button, marginOverride: LayoutConstants.NoMargin), row: row++));
		}

		_ = new ControlLayout(pnlStyles)
		{
			Content = new Grid(
				padding: LayoutConstants.GroupPadding,
				rows:    [.. styleRows],
				content: [.. styleContent]),
		};

		PictureBox picWarning;
		_ = new ControlLayout(_pnlRestartRequiredWarning)
		{
			Content = new Grid(
				columns:
				[
					SizeSpec.Absolute(16),
					SizeSpec.Absolute(3),
					SizeSpec.Everything(),
				],
				content:
				[
					new GridContent(new ControlContent(picWarning = new()
					{
						Margin   = Padding.Empty,
						SizeMode = PictureBoxSizeMode.CenterImage,
						Parent   = _pnlRestartRequiredWarning,
					}), column: 0),
					new GridContent(new ControlContent(new LabelControl()
					{
						AutoSize  = false,
						Margin    = Padding.Empty,
						Text      = "Application must be restarted to apply style changes",
						Parent    = _pnlRestartRequiredWarning,
					}), column: 2),
				]),
		};

		ResumeLayout(false);
		PerformLayout();

		var dpiBindings = new DpiBindings(this);
		dpiBindings.BindImage(picWarning, CommonIcons.Log.Warning);

		_pnlRestartRequiredWarning.Visible = SelectedStyle != GitterApplication.Style;
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(391, 305));

	private void OnThemeRadioButtonCheckedChanged(object? sender, EventArgs e)
	{
		if(sender is IRadioButtonWidget { IsChecked: true } button)
		{
			var style = (IGitterStyle)button.Tag!;
			SelectedStyle = style;
		}
	}

	private IGitterStyle SelectedStyle
	{
		get => _selectedStyle;
		set
		{
			if(_selectedStyle != value)
			{
				_selectedStyle = value;
				_pnlRestartRequiredWarning.Visible = value != GitterApplication.Style;
			}
		}
	}

	public bool Execute()
	{
		GitterApplication.StyleOnNextStartup = SelectedStyle;
		return true;
	}
}
