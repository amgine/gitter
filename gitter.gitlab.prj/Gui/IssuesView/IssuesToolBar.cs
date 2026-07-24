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

namespace gitter.GitLab.Gui;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;
using gitter.Framework.Controls;
using gitter.GitLab.Gui.ListBoxes;

using Resources = gitter.GitLab.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class IssuesToolbar : ToolStrip
{
	private readonly IssuesView _view;
	private readonly ToolStripButton _btnRefresh;
	private readonly ToolStripButton _btnOpen;
	private readonly ToolStripButton _btnClosed;
	private readonly ToolStripButton _btnAll;
	private readonly ToolStripButton _btnScopeAll;
	private readonly ToolStripButton _btnScopeCreated;
	private readonly ToolStripButton _btnScopeAssigned;
	private readonly ToolStripDropDownButton _btnType;
	private readonly ToolStripDropDownButton _btnLabels;
	private readonly ToolStripDropDownButton _btnMilestone;
	private readonly LabelsFilterDropDown _labelsDropDown;
	private bool _labelsChanged;

	public IssuesToolbar(IssuesView view)
	{
		_view = view;

		var dpiBindings = new DpiBindings(this);

		Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, null,
			(_, _) => _view.RefreshContent())
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		});
		dpiBindings.BindImage(_btnRefresh, CommonIcons.Refresh);
		Items.Add(new ToolStripSeparator());

		Items.Add(_btnOpen = new ToolStripButton(Resources.StrOpen)
		{
			Checked     = view.IssueState == Api.IssueState.Opened,
			AutoToolTip = false,
		});
		Items.Add(_btnClosed = new ToolStripButton(Resources.StrClosed)
		{
			Checked     = view.IssueState == Api.IssueState.Closed,
			AutoToolTip = false,
		});
		Items.Add(_btnAll = new ToolStripButton(Resources.StrAll)
		{
			Checked     = view.IssueState == null,
			AutoToolTip = false,
		});
		_btnOpen.Click   += (_, _) => SetState(Api.IssueState.Opened);
		_btnClosed.Click += (_, _) => SetState(Api.IssueState.Closed);
		_btnAll.Click    += (_, _) => SetState(null);

		Items.Add(new ToolStripSeparator());
		Items.Add(_btnScopeAll = new ToolStripButton(Resources.StrAll)
		{
			Checked     = view.IssueScope == null,
			AutoToolTip = false,
		});
		Items.Add(_btnScopeCreated = new ToolStripButton(Resources.StrCreatedByMe)
		{
			Checked     = view.IssueScope == Api.IssueScope.CreatedByMe,
			AutoToolTip = false,
		});
		Items.Add(_btnScopeAssigned = new ToolStripButton(Resources.StrAssignedToMe)
		{
			Checked     = view.IssueScope == Api.IssueScope.AssignedToMe,
			AutoToolTip = false,
		});
		_btnScopeAll.Click      += (_, _) => SetScope(null);
		_btnScopeCreated.Click  += (_, _) => SetScope(Api.IssueScope.CreatedByMe);
		_btnScopeAssigned.Click += (_, _) => SetScope(Api.IssueScope.AssignedToMe);

		Items.Add(new ToolStripSeparator());
		Items.Add(_btnType = new ToolStripDropDownButton(Resources.StrType));
		_btnType.DropDownOpening += OnTypeDropDownOpening;
		UpdateTypeCaption();

		Items.Add(_btnLabels = new ToolStripDropDownButton(Resources.StrLabels)
		{
			DropDown = new Popup(_labelsDropDown = new LabelsFilterDropDown())
			{
				Resizable = false,
			},
			ToolTipText = Resources.StrLabels,
		});
		_labelsDropDown.SelectionChanged += (_, _) => _labelsChanged = true;
		_btnLabels.DropDownOpening += OnLabelsDropDownOpening;
		_btnLabels.DropDownClosed  += OnLabelsDropDownClosed;
		UpdateLabelsCaption();

		Items.Add(_btnMilestone = new ToolStripDropDownButton(Resources.StrMilestone));
		_btnMilestone.DropDownOpening += OnMilestoneDropDownOpening;
	}

	public ToolStripButton RefreshButton => _btnRefresh;

	private void SetState(Api.IssueState? state)
	{
		_btnOpen.Checked   = state == Api.IssueState.Opened;
		_btnClosed.Checked = state == Api.IssueState.Closed;
		_btnAll.Checked    = state == null;
		_view.IssueState   = state;
	}

	private void SetScope(Api.IssueScope? scope)
	{
		_btnScopeAll.Checked      = scope == null;
		_btnScopeCreated.Checked  = scope == Api.IssueScope.CreatedByMe;
		_btnScopeAssigned.Checked = scope == Api.IssueScope.AssignedToMe;
		_view.IssueScope          = scope;
	}

	private void OnTypeDropDownOpening(object? sender, EventArgs e)
	{
		_btnType.DropDownItems.Clear();

		AddTypeItem(Resources.StrAny, null);
		_btnType.DropDownItems.Add(new ToolStripSeparator());
		foreach(var type in Api.WorkItemTypes.Filterable)
		{
			AddTypeItem(IssueTypeColumn.GetDisplayName(type), type);
		}
	}

	private void AddTypeItem(string display, Api.WorkItemType? value)
	{
		var item = new ToolStripMenuItem(display)
		{
			Checked = _view.IssueType == value,
			Tag     = value,
		};
		item.Click += (_, _) =>
		{
			_view.IssueType = (Api.WorkItemType?)item.Tag;
			UpdateTypeCaption();
		};
		_btnType.DropDownItems.Add(item);
	}

	private void UpdateTypeCaption()
	{
		_btnType.Text = _view.IssueType is { } type
			? $"{Resources.StrType}: {IssueTypeColumn.GetDisplayName(type)}"
			: Resources.StrType;
	}

	private void OnLabelsDropDownOpening(object? sender, EventArgs e)
	{
		_labelsChanged = false;
		_labelsDropDown.SetSelectedLabels(_view.SelectedLabels);
		_labelsDropDown.SetLabels(_view.LabelRegistry?.All);
	}

	private void OnLabelsDropDownClosed(object? sender, EventArgs e)
	{
		if(!_labelsChanged) return;

		_labelsChanged = false;
		_view.SetSelectedLabels(new List<string>(_labelsDropDown.SelectedLabels));
		UpdateLabelsCaption();
	}

	private void UpdateLabelsCaption()
	{
		var count = _view.SelectedLabels.Count;
		_btnLabels.Text = count > 0 ? $"{Resources.StrLabels} ({count})" : Resources.StrLabels;
	}

	private void OnMilestoneDropDownOpening(object? sender, EventArgs e)
	{
		_btnMilestone.DropDownItems.Clear();

		var seen = new HashSet<string>(StringComparer.Ordinal);
		foreach(CustomListBoxItem item in _view.IssuesListBoxItems)
		{
			if(item is IssueListItem { DataContext.Milestone.Title: { } t }) seen.Add(t);
		}

		AddMilestoneItem(Resources.StrAny,  null);
		AddMilestoneItem(Resources.StrNone, "None");
		_btnMilestone.DropDownItems.Add(new ToolStripSeparator());
		foreach(var title in seen) AddMilestoneItem(title, title);
		UpdateMilestoneCaption();
	}

	private void AddMilestoneItem(string display, string? value)
	{
		var item = new ToolStripMenuItem(display)
		{
			Checked = string.Equals(_view.SelectedMilestone, value, StringComparison.Ordinal),
			Tag     = value,
		};
		item.Click += (_, _) =>
		{
			_view.SelectedMilestone = (string?)item.Tag;
			UpdateMilestoneCaption();
		};
		_btnMilestone.DropDownItems.Add(item);
	}

	private void UpdateMilestoneCaption()
	{
		_btnMilestone.Text = _view.SelectedMilestone is { Length: > 0 }
			? $"{Resources.StrMilestone}: {_view.SelectedMilestone}"
			: Resources.StrMilestone;
	}
}
