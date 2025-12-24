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

namespace gitter.Git.Gui.Views;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.Framework.Layout;
using gitter.Framework.Mvc;
using gitter.Framework.Mvc.WinForms;
using gitter.Git.Gui.Controls;

[DesignerCategory("")]
partial class HistoryFilterDropDown : UserControl
{
	readonly struct DialogControls
	{
		public readonly TextBox _txtSearch;
		public readonly ReferencesListBox _lstReferences;
		public readonly IRadioButtonWidget radioButton1;
		public readonly IRadioButtonWidget radioButton2;
		public readonly IRadioButtonWidget radioButton3;

		public DialogControls(IGitterStyle style)
		{
			style ??= GitterApplication.Style;

			var rbf = style.RadioButtonFactory;
			_lstReferences = new()
			{
				Style               = style,
				DisableContextMenus = true,
				HeaderStyle         = HeaderStyle.Hidden,
				ShowCheckBoxes      = true,
				ShowTreeLines       = true,
			};
			_txtSearch = new();
			radioButton1 = rbf.Create();
			radioButton2 = rbf.Create();
			radioButton3 = rbf.Create();
		}

		public void Localize()
		{
			radioButton1.Text = "All references";
			radioButton2.Text = GitConstants.HEAD;
			radioButton3.Text = "Only selected references:";
#if NETCOREAPP
			_txtSearch.PlaceholderText = "Filter";
#endif
		}

		public void Layout(Control parent)
		{
			var filterDec = new TextBoxDecorator(_txtSearch);

			_ = new ControlLayout(parent)
			{
				Content = new Grid(
					padding: DpiBoundValue.Padding(new(4)),
					rows:
					[
						LayoutConstants.RadioButtonRowHeight,
						LayoutConstants.RadioButtonRowHeight,
						LayoutConstants.TextInputRowHeight,
						SizeSpec.Everything(),
					],
					content:
					[
						new GridContent(new Grid(
							columns:
							[
								SizeSpec.Absolute(130),
								SizeSpec.Everything(),
							],
							content:
							[
								new GridContent(new WidgetContent(radioButton1, marginOverride: LayoutConstants.NoMargin), column: 0),
								new GridContent(new WidgetContent(radioButton2, marginOverride: LayoutConstants.NoMargin), column: 1)
							]), row: 0),
						new GridContent(new WidgetContent (radioButton3,   marginOverride: LayoutConstants.NoMargin),      row: 1),
						new GridContent(new ControlContent(filterDec,      marginOverride: LayoutConstants.TextBoxMargin), row: 2),
						new GridContent(new ControlContent(_lstReferences, marginOverride: LayoutConstants.NoMargin),      row: 3),
					]),
			};

			var tabIndex = 0;
			radioButton1.TabIndex = tabIndex++;
			radioButton2.TabIndex = tabIndex++;
			radioButton3.TabIndex = tabIndex++;
			filterDec.TabIndex = tabIndex++;
			_lstReferences.TabIndex = tabIndex++;

			radioButton1.Parent = parent;
			radioButton2.Parent = parent;
			radioButton3.Parent = parent;
			filterDec.Parent = parent;
			_lstReferences.Parent = parent;
		}
	}

	private readonly DialogControls _controls;
	private Repository? _repository;
	private LogOptions? _logOptions;

	public IUserInputSource<string?> Search { get; }

	public HistoryFilterDropDown()
	{
		Name = nameof(HistoryFilterDropDown);

		SuspendLayout();
		AutoScaleDimensions = Dpi.Default;
		AutoScaleMode       = AutoScaleMode.Dpi;
		BorderStyle         = BorderStyle.FixedSingle;
		MaximumSize         = new(241, 500);
		MinimumSize         = new(241, 279);
		Size                = new(239, 279);
		_controls = new(GitterApplication.Style);
		_controls.Localize();
		_controls.Layout(this);
		VisibleChanged += HistoryFilterDropDown_VisibleChanged;
		ResumeLayout(false);
		PerformLayout();

		if(LicenseManager.UsageMode == LicenseUsageMode.Runtime)
		{
			Font = GitterApplication.FontManager.UIFont;
			BackColor = GitterApplication.Style.Colors.Window;
			ForeColor = GitterApplication.Style.Colors.WindowText;
		}
		else
		{
			Font      = SystemFonts.MessageBoxFont;
			BackColor = SystemColors.Window;
		}
		_controls._txtSearch.TextChanged += _txtSearch_TextChanged;
		_controls.radioButton1.IsCheckedChanged += OnFilterTypeCheckedChanged;
		_controls.radioButton2.IsCheckedChanged += OnFilterTypeCheckedChanged;
		_controls.radioButton3.IsCheckedChanged += OnFilterTypeCheckedChanged;

		Search = new TextBoxInputSource(_controls._txtSearch);
	}

	/// <inheritdoc/>
	protected override bool ScaleChildren => false;

	public LogOptions? LogOptions
	{
		get => _logOptions;
		set
		{
			if(_logOptions == value) return;

			_logOptions = value;
			if(value is not null)
			{
				var radio = value.Filter switch
				{
					LogReferenceFilter.All     => _controls.radioButton1,
					LogReferenceFilter.HEAD    => _controls.radioButton2,
					LogReferenceFilter.Allowed => _controls.radioButton3,
					_ => default,
				};
				if(radio is not null) radio.IsChecked = true;
			}
			_controls._lstReferences.ItemCheckedChanged -= OnItemCheckedChanged;
			UpdateCheckStatuses();
			_controls._lstReferences.ItemCheckedChanged += OnItemCheckedChanged;
		}
	}

	private void OnItemCheckedChanged(object? sender, ItemEventArgs e)
	{
		if(_logOptions is null) return;

		if(e.Item is IRevisionPointerListItem item && item.RevisionPointer is Reference reference)
		{
			if(e.Item.IsChecked)
			{
				_logOptions.AllowReference(reference);
			}
			else
			{
				_logOptions.DisallowReference(reference);
			}
		}
	}

	private void UpdateCheckStatuses()
	{
		if(_logOptions is not null)
		{
			UpdateCheckStatuses(_controls._lstReferences.Items);
		}
	}

	private void UpdateCheckStatuses(CustomListBoxItemsCollection items)
	{
		foreach(var item in items)
		{
			if(item is IRevisionPointerListItem { RevisionPointer: Reference reference })
			{
				item.IsChecked = _logOptions is not null
					&& _logOptions.AllowedReferences.Contains(reference);
			}
			UpdateCheckStatuses(item.Items);
		}
	}

	public Repository? Repository
	{
		get => _repository;
		set
		{
			if(_repository == value) return;

			_repository = value;
			LoadReferences(value, null);
		}
	}

	private void OnFilterTypeCheckedChanged(object? sender, EventArgs e)
	{
		if(sender is not IRadioButtonWidget { IsChecked: true }) return;
		if(_logOptions is null) return;

		if(_controls.radioButton1.IsChecked) _logOptions.Filter = LogReferenceFilter.All;
		if(_controls.radioButton2.IsChecked) _logOptions.Filter = LogReferenceFilter.HEAD;
		if(_controls.radioButton3.IsChecked) _logOptions.Filter = LogReferenceFilter.Allowed;
	}

	private void LoadReferences(Repository? repository, string? filter)
	{
		_controls._lstReferences.ItemCheckedChanged -= OnItemCheckedChanged;
		if(string.IsNullOrEmpty(filter))
		{
			_controls._lstReferences.LoadData(repository);
		}
		else
		{
			_controls._lstReferences.LoadData(repository, ReferenceType.Reference, true, true, x =>
			{
				if(x is Reference reference) return reference.Name.Contains(filter);
				return x.FullName.Contains(filter);
			});
		}
		_controls._lstReferences.EnableCheckboxes();
		_controls._lstReferences.ExpandAll();
		UpdateCheckStatuses();
		_controls._lstReferences.ItemCheckedChanged += OnItemCheckedChanged;
	}

	private void _txtSearch_TextChanged(object? sender, EventArgs e)
	{
		if(_controls._lstReferences.Items.Count == 0) return;
		LoadReferences(Repository, Search.Value);
	}

	private void HistoryFilterDropDown_VisibleChanged(object? sender, EventArgs e)
	{
		Search.Value = string.Empty;
	}
}
