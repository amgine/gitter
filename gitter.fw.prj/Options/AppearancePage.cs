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
	private readonly RadioButton _radGdi;
	private readonly RadioButton _radGdiPlus;
	private IGitterStyle _selectedStyle;

	public AppearancePage()
		: base(PropertyPageFactory.AppearanceGroupGuid)
	{
		SuspendLayout();

		Panel pnlTextRenderers;
		Panel pnlStyles;

		var radioButtonRowSize = SizeSpec.Absolute(23);

		_selectedStyle = GitterApplication.StyleOnNextStartup;

		_ = new ControlLayout(this)
		{
			Content = new Grid(
				rows:new[]
				{
					SizeSpec.Absolute(19),
					SizeSpec.Absolute(50),
					SizeSpec.Absolute(19),
					SizeSpec.Everything(),
					SizeSpec.Absolute(23),
				},
				content: new[]
				{
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
					}), row: 1),
					new GridContent(new ControlContent(new GroupSeparator()
					{
						Margin = Padding.Empty,
						Text   = "Application theme",
						Parent = this,
					}), row: 2),
					new GridContent(new ControlContent(pnlStyles = new()
					{
						Margin = Padding.Empty,
						Parent = this,
					}), row: 3),
					new GridContent(new ControlContent(_pnlRestartRequiredWarning = new()
					{
						Margin = Padding.Empty,
						Parent = this,
					}), row: 4),
				}),
		};

		_ = new ControlLayout(pnlTextRenderers)
		{
			Content = new Grid(
				padding: DpiBoundValue.Padding(new(3, 0, 0, 0)),
				rows: new[]
				{
					radioButtonRowSize,
					radioButtonRowSize,
					SizeSpec.Everything(),
				},
				content: new[]
				{
					new GridContent(new ControlContent(_radGdi = new()
					{
						Margin    = Padding.Empty,
						Text      = "GDI",
						Checked   = GitterApplication.TextRenderer == GitterApplication.GdiTextRenderer,
						FlatStyle = FlatStyle.System,
						Parent    = pnlTextRenderers,
					}), row: 0),
					new GridContent(new ControlContent(_radGdiPlus = new()
					{
						Margin    = Padding.Empty,
						Text      = "GDI+",
						Checked   = GitterApplication.TextRenderer == GitterApplication.GdiPlusTextRenderer,
						FlatStyle = FlatStyle.System,
						Parent    = pnlTextRenderers,
					}), row: 1),
				}),
		};

		var styleRows    = new List<ISizeSpec>();
		var styleContent = new List<GridContent>();
		var row = 0;
		foreach(var style in GitterApplication.Styles)
		{
			styleRows.Add(radioButtonRowSize);
			var button = new RadioButton
			{
				Margin    = Padding.Empty,
				Text      = style.DisplayName,
				FlatStyle = FlatStyle.System,
				Tag       = style,
				Checked   = style == SelectedStyle,
				Parent    = pnlStyles,
			};
			button.CheckedChanged += OnThemeRadioButtonCheckedChanged;
			styleContent.Add(new GridContent(new ControlContent(button), row: row++));
		}

		_ = new ControlLayout(pnlStyles)
		{
			Content = new Grid(
				padding: DpiBoundValue.Padding(new(3, 0, 0, 0)),
				rows:    styleRows.ToArray(),
				content: styleContent.ToArray()),
		};

		PictureBox picWarning;
		_ = new ControlLayout(_pnlRestartRequiredWarning)
		{
			Content = new Grid(
				columns: new[]
				{
					SizeSpec.Absolute(16),
					SizeSpec.Absolute(3),
					SizeSpec.Everything(),
				},
				content: new[]
				{
					new GridContent(new ControlContent(picWarning = new()
					{
						Margin   = Padding.Empty,
						SizeMode = PictureBoxSizeMode.CenterImage,
						Parent   = _pnlRestartRequiredWarning,
					}), column: 0),
					new GridContent(new ControlContent(new Label()
					{
						AutoSize  = false,
						TextAlign = ContentAlignment.MiddleLeft,
						Margin    = Padding.Empty,
						Text      = "Application must be restarted to apply style changes",
						Parent    = _pnlRestartRequiredWarning,
					}), column: 2),
				}),
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

	private void OnThemeRadioButtonCheckedChanged(object sender, EventArgs e)
	{
		var radioButton = (RadioButton)sender;
		if(radioButton.Checked)
		{
			var style = (IGitterStyle)radioButton.Tag;
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
		if(_radGdi.Checked)
		{
			GitterApplication.TextRenderer = GitterApplication.GdiTextRenderer;
		}
		else if(_radGdiPlus.Checked)
		{
			GitterApplication.TextRenderer = GitterApplication.GdiPlusTextRenderer;
		}
		GitterApplication.StyleOnNextStartup = SelectedStyle;
		return true;
	}
}
