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

namespace gitter.Framework.Options;

using System;
using System.ComponentModel;
using System.Drawing;

[ToolboxItem(false)]
public partial class ColorsPage : PropertyPage
{
	public static readonly new Guid Guid = new("AD2A7C07-6E10-4F0D-B471-F6DA58638660");

	public ColorsPage()
		: base(Guid)
	{
	}

	public override IDpiBoundValue<Size> ScalableSize { get; } = DpiBoundValue.Size(new(448, 375));
}
