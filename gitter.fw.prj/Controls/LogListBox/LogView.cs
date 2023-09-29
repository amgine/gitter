﻿#region Copyright Notice
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Resources = gitter.Framework.Properties.Resources;

[ToolboxItem(false)]
[DesignerCategory("")]
internal sealed class LogView : ViewBase
{
	/// <summary>Initializes a new instance of the <see cref="LogView"/> class.</summary>
	public LogView(IWorkingEnvironment environment)
		: base(LogViewFactory.Guid, environment)
	{
		Height = 200;

		_ = new LogListBox()
		{
			BorderStyle = BorderStyle.None,
			Dock = DockStyle.Fill,
			Parent = this,
		};

		Text = Resources.StrLog;
	}

	private static readonly IDpiBoundValue<Size> _defaultScalableSize = DpiBoundValue.Size(new(555, 200));

	public override IDpiBoundValue<Size> DefaultScalableSize => _defaultScalableSize;

	public override IImageProvider ImageProvider { get; } = new ScaledImageProvider(CachedResources.ScaledBitmaps, @"log.view");
}
