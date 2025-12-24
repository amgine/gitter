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

namespace gitter.Git.Gui.Controls;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;

using Resources = gitter.Git.Gui.Properties.Resources;

/// <summary>Extender for <see cref="GraphColumn"/>.</summary>
[ToolboxItem(false)]
class GraphColumnExtender : ExtenderBase
{
	private ICheckBoxWidget? _chkShowColors;
	private ICheckBoxWidget? _chkFillBackground;
	private bool _disableEvents;

	/// <summary>Create <see cref="GraphColumnExtender"/>.</summary>
	/// <param name="column">Related column.</param>
	public GraphColumnExtender(GraphColumn column)
		: base(column)
	{
		SuspendLayout();
		Name = nameof(GraphColumnExtender);
		Size = new(148, 50);
		ResumeLayout();

		CreateControls();
		SubscribeToColumnEvents();
	}

	/// <inheritdoc/>
	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(148, 50));

	/// <inheritdoc/>
	protected override void Dispose(bool disposing)
	{
		if(disposing)
		{
			DisposableUtility.Dispose(ref _chkShowColors);
			DisposableUtility.Dispose(ref _chkFillBackground);
			UnsubscribeFromColumnEvents();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc/>
	protected override void OnStyleChanged()
	{
		base.OnStyleChanged();
		CreateControls();
	}

	private void CreateControls()
	{
		var conv = DpiConverter.FromDefaultTo(Dpi.FromControl(this));

		_chkShowColors?.Dispose();
		_chkShowColors = Style.CheckBoxFactory.Create();
		_chkShowColors.IsChecked = Column.ShowColors;
		_chkShowColors.IsCheckedChanged += OnShowColorsCheckedChanged;
		_chkShowColors.Text = Resources.StrShowColors;

		_chkFillBackground?.Dispose();
		_chkFillBackground = Style.CheckBoxFactory.Create();
		_chkFillBackground.IsChecked = Column.FillBackground;
		_chkFillBackground.IsCheckedChanged += OnFillBackgroundCheckedChanged;
		_chkFillBackground.Text = Resources.StrFillBackground;

		var noMargin = DpiBoundValue.Constant(Padding.Empty);
		_ = new ControlLayout(this)
		{
			Content = new Grid(
				padding: DpiBoundValue.Padding(new Padding(6, 2, 6, 2)),
				rows:
				[
					SizeSpec.Absolute(23),
					SizeSpec.Absolute(23),
				],
				content:
				[
					new GridContent(new ControlContent(_chkShowColors.Control,     marginOverride: noMargin), row: 0),
					new GridContent(new ControlContent(_chkFillBackground.Control, marginOverride: noMargin), row: 1),
				]),
		};

		_chkShowColors.Control.Parent     = this;
		_chkFillBackground.Control.Parent = this;
	}

	private void SubscribeToColumnEvents()
	{
		Column.ShowColorsChanged     += OnColumnShowColorsChanged;
		Column.FillBackgroundChanged += OnColumnFillBackgroundChanged;
	}

	private void UnsubscribeFromColumnEvents()
	{
		Column.ShowColorsChanged     -= OnShowColorsCheckedChanged;
		Column.FillBackgroundChanged -= OnColumnFillBackgroundChanged;
	}

	public new GraphColumn Column => (GraphColumn)base.Column;

	public bool ShowColors
	{
		get => _chkShowColors is not null ? _chkShowColors.IsChecked : Column.ShowColors;
		private set
		{
			if(_chkShowColors is not null)
			{
				_disableEvents = true;
				_chkShowColors.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	public bool FillBackground
	{
		get => _chkFillBackground is not null ? _chkFillBackground.IsChecked : Column.FillBackground;
		private set
		{
			if(_chkFillBackground is not null)
			{
				_disableEvents = true;
				_chkFillBackground.IsChecked = value;
				_disableEvents = false;
			}
		}
	}

	private void OnColumnShowColorsChanged(object? sender, EventArgs e)
	{
		ShowColors = Column.ShowColors;
	}

	private void OnShowColorsCheckedChanged(object? sender, EventArgs e)
	{
		if(_disableEvents) return;

		_disableEvents = true;
		Column.ShowColors = sender is ICheckBoxWidget { IsChecked: true };
		_disableEvents = false;
	}

	private void OnColumnFillBackgroundChanged(object? sender, EventArgs e)
	{
		FillBackground = Column.FillBackground;
	}

	private void OnFillBackgroundCheckedChanged(object? sender, EventArgs e)
	{
		if(_disableEvents) return;

		_disableEvents = true;
		Column.FillBackground = sender is ICheckBoxWidget { IsChecked: true };
		_disableEvents = false;
	}
}
