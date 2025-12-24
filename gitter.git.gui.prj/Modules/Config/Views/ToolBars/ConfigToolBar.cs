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
using System.Windows.Forms;

using gitter.Framework;

using gitter.Git.Gui.Dialogs;

using Resources = gitter.Git.Gui.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class ConfigToolBar : ToolStrip
{
	private readonly ConfigView _configView;
	private readonly ToolStripButton _btnConfigAdd;
	private readonly DpiBindings _dpiBindings;

	public ConfigToolBar(ConfigView configView)
	{
		Verify.Argument.IsNotNull(configView);

		_dpiBindings = new DpiBindings(this);
		var factory = new GuiItemFactory(_dpiBindings);

		_configView = configView;
		Items.Add(factory.CreateRefreshContentButton(configView));
		Items.Add(new ToolStripSeparator());
		Items.Add(_btnConfigAdd = new ToolStripButton(Resources.StrAddParameter, default, OnAddParameterButtonClick));

		_dpiBindings.BindImage(_btnConfigAdd, Icons.ConfigAdd);
	}

	private void OnAddParameterButtonClick(object? sender, EventArgs e)
	{
		if(_configView.Repository is null) return;

		using var dialog = new AddParameterDialog(_configView.WorkingEnvironment, _configView.Repository);
		dialog.Run(_configView);
	}
}
