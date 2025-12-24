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
using System.Drawing;
using System.Windows.Forms;

public interface IWidget : IDisposable
{
	Control Control { get; }

	bool Enabled { get; set; }

	Control? Parent { get; set; }

	Rectangle Bounds { get; set; }

	Padding Margin { get; set; }

	Padding Padding { get; set; }

	DockStyle Dock { get; set; }

	AnchorStyles Anchor { get; set; }

	int TabIndex { get; set; }

	object? Tag { get; set; }

	bool IsDisposed { get; }
}
