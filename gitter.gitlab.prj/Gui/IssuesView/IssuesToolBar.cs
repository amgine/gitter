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
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

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
			Checked = view.IssueState == Api.IssueState.Opened,
			AutoToolTip = false,
		});
		Items.Add(_btnClosed = new ToolStripButton(Resources.StrClosed)
		{
			Checked = view.IssueState == Api.IssueState.Closed,
			AutoToolTip = false,
		});
		Items.Add(_btnAll = new ToolStripButton(Resources.StrAll)
		{
			Checked = view.IssueState == null,
			AutoToolTip = false,
		});

		_btnOpen.Click += (_, _) =>
		{
			_btnClosed.Checked = false;
			_btnAll.Checked    = false;
			_btnOpen.Checked   = true;
			_view.IssueState   = Api.IssueState.Opened;
		};
		_btnClosed.Click += (_, _) =>
		{
			_btnAll.Checked    = false;
			_btnOpen.Checked   = false;
			_btnClosed.Checked = true;
			_view.IssueState   = Api.IssueState.Closed;
		};
		_btnAll.Click += (_, _) =>
		{
			_btnClosed.Checked = false;
			_btnOpen.Checked   = false;
			_btnAll.Checked    = true;
			_view.IssueState   = null;
		};
	}

	public ToolStripButton RefreshButton => _btnRefresh;
}
