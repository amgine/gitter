#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2021  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
internal sealed class PipelinesToolbar : ToolStrip
{
	private readonly PipelinesView _view;
	private readonly ToolStripButton _btnRefresh;
	private readonly ToolStripButton _btnAll;
	private readonly ToolStripButton _btnFinished;
	private readonly ToolStripButton _btnBranches;
	private readonly ToolStripButton _btnTags;

	public PipelinesToolbar(PipelinesView view)
	{
		Verify.Argument.IsNotNull(view);

		_view = view;

		var dpiBindings = new DpiBindings(this);

		Items.Add(_btnRefresh = new ToolStripButton(Resources.StrRefresh, null,
			(_, _) => _view.RefreshContent())
		{
			DisplayStyle = ToolStripItemDisplayStyle.Image,
		});
		dpiBindings.BindImage(_btnRefresh, CommonIcons.Refresh);

		Items.Add(new ToolStripSeparator());

		Items.Add(_btnAll = new ToolStripButton(Resources.StrAll)
		{
			Checked = !view.Scope.HasValue,
			AutoToolTip = false,
		});
		Items.Add(_btnFinished = new ToolStripButton(Resources.StrFinished)
		{
			Checked = view.Scope == Api.PipelineScope.Finished,
			AutoToolTip = false,
		});
		Items.Add(_btnBranches = new ToolStripButton(Resources.StrBranches)
		{
			Checked = view.Scope == Api.PipelineScope.Branches,
			AutoToolTip = false,
		});
		Items.Add(_btnTags = new ToolStripButton(Resources.StrTags)
		{
			Checked = view.Scope == Api.PipelineScope.Tags,
			AutoToolTip = false,
		});

		_btnAll.Click += (_, _) =>
		{
			_btnAll.Checked      = true;
			_btnFinished.Checked = false;
			_btnBranches.Checked = false;
			_btnTags.Checked     = false;
			_view.Scope = null;
		};

		_btnFinished.Click += (_, _) =>
		{
			_btnAll.Checked      = false;
			_btnFinished.Checked = true;
			_btnBranches.Checked = false;
			_btnTags.Checked     = false;
			_view.Scope = Api.PipelineScope.Finished;
		};

		_btnBranches.Click += (_, _) =>
		{
			_btnAll.Checked      = false;
			_btnFinished.Checked = false;
			_btnBranches.Checked = true;
			_btnTags.Checked     = false;
			_view.Scope = Api.PipelineScope.Branches;
		};

		_btnTags.Click += (_, _) =>
		{
			_btnAll.Checked      = false;
			_btnFinished.Checked = false;
			_btnBranches.Checked = false;
			_btnTags.Checked     = true;
			_view.Scope = Api.PipelineScope.Tags;
		};
	}

	public ToolStripButton RefreshButton => _btnRefresh;
}
