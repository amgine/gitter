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

namespace gitter.Framework.Controls;

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;

[DesignerCategory("")]
class ViewSwitcher : Form
{
	private void InitializeComponent()
	{
		this.SuspendLayout();
		// 
		// ViewSwitcher
		// 
		this.BackColor = System.Drawing.SystemColors.Window;
		this.ClientSize = new System.Drawing.Size(733, 386);
		this.ControlBox = false;
		this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.Name = "ViewSwitcher";
		this.ShowIcon = false;
		this.ShowInTaskbar = false;
		this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.ResumeLayout(false);

	}
}
