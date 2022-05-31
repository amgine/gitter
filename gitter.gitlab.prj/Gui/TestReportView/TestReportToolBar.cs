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

namespace gitter.GitLab.Gui;

using System;
using System.ComponentModel;
using System.Windows.Forms;

using gitter.Framework;

using Resources = gitter.GitLab.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class TestReportToolBar : ToolStrip
{
	private readonly TestReportView _view;
	private readonly ToolStripButton _btnRefresh;
	private readonly ToolStripButton _btnSuccess;
	private readonly ToolStripButton _btnSkipped;
	private readonly ToolStripButton _btnFailed;
	private readonly ToolStripButton _btnErrors;

	public TestReportToolBar(TestReportView view)
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

		Items.Add(_btnSuccess = new ToolStripButton("0")
		{
			CheckOnClick = true,
			Checked      = true,
			AutoToolTip  = false,
			ToolTipText  = Resources.StrSuccessTests,
		});
		dpiBindings.BindImage(_btnSuccess, Icons.TestSuccess);

		Items.Add(_btnSkipped = new ToolStripButton("0")
		{
			CheckOnClick = true,
			Checked      = true,
			AutoToolTip  = false,
			ToolTipText  = Resources.StrSkippedTests,
		});
		dpiBindings.BindImage(_btnSkipped, Icons.TestSkipped);

		Items.Add(_btnFailed = new ToolStripButton("0")
		{
			CheckOnClick = true,
			Checked      = true,
			AutoToolTip  = false,
			ToolTipText  = Resources.StrFailedTests,
		});
		dpiBindings.BindImage(_btnFailed, Icons.TestFailed);

		Items.Add(_btnErrors = new ToolStripButton("0")
		{
			CheckOnClick = true,
			Checked      = true,
			AutoToolTip  = false,
			ToolTipText  = Resources.StrErrors,
		});
		dpiBindings.BindImage(_btnErrors, Icons.TestError);


		Items.Add(ExpandAll = new ToolStripButton(Resources.StrExpandAll)
		{
			Alignment    = ToolStripItemAlignment.Right,
			DisplayStyle = ToolStripItemDisplayStyle.Text,
			AutoToolTip  = false,
		});
		Items.Add(CollapseAll = new ToolStripButton(Resources.StrCollapseAll)
		{
			Alignment    = ToolStripItemAlignment.Right,
			DisplayStyle = ToolStripItemDisplayStyle.Text,
			AutoToolTip  = false,
		});
	}

	public ToolStripButton Success => _btnSuccess;

	public ToolStripButton Skipped => _btnSkipped;

	public ToolStripButton Failed => _btnFailed;

	public ToolStripButton Errors => _btnErrors;

	public ToolStripButton ExpandAll { get; }

	public ToolStripButton CollapseAll { get; }
}
