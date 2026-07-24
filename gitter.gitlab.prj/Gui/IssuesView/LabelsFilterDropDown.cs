#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2026  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;

using Label     = gitter.GitLab.Api.Label;
using Resources = gitter.GitLab.Properties.Resources;

[DesignerCategory("")]
sealed class LabelsFilterDropDown : UserControl
{
	readonly struct DropDownControls
	{
		public readonly TextBox _txtFilter;
		public readonly LabelsListBox _lstLabels;
		public readonly LinkLabel _lnkClear;

		public DropDownControls()
		{
			_txtFilter = new();
			_lstLabels = new();
			_lnkClear  = new()
			{
				LinkColor         = GitterApplication.Style.Colors.HyperlinkText,
				ActiveLinkColor   = GitterApplication.Style.Colors.HyperlinkTextHotTrack,
				DisabledLinkColor = GitterApplication.Style.Colors.GrayText,
				TextAlign         = ContentAlignment.MiddleLeft,
				Padding           = default,
			};
		}

		public void Localize()
		{
			_lnkClear.Text = Resources.StrClearSelection;
#if NETCOREAPP
			_txtFilter.PlaceholderText = Resources.StrFilter;
#endif
		}

		public void Layout(Control parent)
		{
			var filterDec = new TextBoxDecorator(_txtFilter);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					padding: DpiBoundValue.Padding(new(4)),
					rows:
					[
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
						LayoutConstants.LabelRowHeight,
					],
					content:
					[
						new GridContent(new ControlContent(filterDec,  marginOverride: LayoutConstants.TextBoxMargin), row: 0),
						new GridContent(new ControlContent(_lstLabels, marginOverride: LayoutConstants.NoMargin),      row: 1),
						new GridContent(new ControlContent(_lnkClear,  marginOverride: LayoutConstants.NoMargin),      row: 2),
					]),
			};

			var tabIndex = 0;
			filterDec.TabIndex = tabIndex++;
			_lstLabels.TabIndex = tabIndex++;
			_lnkClear.TabIndex = tabIndex++;

			filterDec.Parent  = parent;
			_lstLabels.Parent = parent;
			_lnkClear.Parent  = parent;
		}
	}

	private readonly DropDownControls _controls;
	private readonly LabelsFilterModel _model = new();
	private bool _suppressCheckEvents;

	public event EventHandler? SelectionChanged;

	public IUserInputSource<string?> Filter { get; }

	public LabelsFilterDropDown()
	{
		Name = nameof(LabelsFilterDropDown);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		BorderStyle         = BorderStyle.FixedSingle;
		MinimumSize         = new(240, 260);
		MaximumSize         = new(240, 420);
		Size                = new(240, 320);
		_controls = new();
		_controls.Localize();
		_controls.Layout(this);
		ResumeLayout(performLayout: false);
		PerformLayout();

		if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
		{
			Font      = GitterApplication.FontManager.UIFont;
			BackColor = GitterApplication.Style.Colors.Window;
			ForeColor = GitterApplication.Style.Colors.WindowText;
		}
		else
		{
			Font      = SystemFonts.MessageBoxFont;
			BackColor = SystemColors.Window;
		}

		Filter = new TextBoxInputSource(_controls._txtFilter);

		_controls._txtFilter.TextChanged   += OnFilterTextChanged;
		_controls._lstLabels.ItemCheckedChanged += OnItemCheckedChanged;
		_controls._lnkClear.LinkClicked    += OnClearLinkClicked;

		VisibleChanged += OnVisibleChanged;
	}

	protected override bool ScaleChildren => false;

	public void SetLabels(IReadOnlyList<Label>? labels)
	{
		_model.Labels = labels!;
		Rebuild();
	}

	public IReadOnlyCollection<string> SelectedLabels => _model.Selected;

	public void SetSelectedLabels(IEnumerable<string>? labels)
	{
		_model.SetSelected(labels);
		ApplyCheckStates();
		UpdateClearLinkState();
	}

	private void Rebuild()
	{
		_suppressCheckEvents = true;
		_controls._lstLabels.BeginUpdate();
		try
		{
			_controls._lstLabels.Items.Clear();
			foreach(var label in _model.GetVisibleLabels(_controls._txtFilter.Text))
			{
				_controls._lstLabels.Items.Add(new LabelListItem(label)
				{
					IsChecked = _model.IsSelected(label.Name),
				});
			}
		}
		finally
		{
			_controls._lstLabels.EndUpdate();
			_suppressCheckEvents = false;
		}
		UpdateClearLinkState();
	}

	private void ApplyCheckStates()
	{
		_suppressCheckEvents = true;
		try
		{
			foreach(var item in _controls._lstLabels.Items)
			{
				if(item is LabelListItem label)
				{
					label.IsChecked = _model.IsSelected(label.DataContext.Name);
				}
			}
		}
		finally
		{
			_suppressCheckEvents = false;
		}
	}

	private void OnFilterTextChanged(object? sender, EventArgs e) => Rebuild();

	private void OnItemCheckedChanged(object? sender, ItemEventArgs e)
	{
		if(_suppressCheckEvents) return;
		if(e.Item is not LabelListItem label) return;

		if(!_model.Select(label.DataContext.Name, e.Item.IsChecked)) return;

		UpdateClearLinkState();
		SelectionChanged?.Invoke(this, EventArgs.Empty);
	}

	private void OnClearLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
	{
		if(!_model.Clear()) return;

		ApplyCheckStates();
		UpdateClearLinkState();
		SelectionChanged?.Invoke(this, EventArgs.Empty);
	}

	private void UpdateClearLinkState()
	{
		var count = _model.SelectedCount;
		_controls._lnkClear.Text = count == 0
			? Resources.StrClearSelection
			: $"{Resources.StrClearSelection} ({count})";
		_controls._lnkClear.Links[0].Enabled = count != 0;
	}

	private void OnVisibleChanged(object? sender, EventArgs e)
	{
		if(!Visible) return;

		Filter.Value = string.Empty;
		if(IsHandleCreated) BeginInvoke(_controls._txtFilter.Focus);
	}
}
