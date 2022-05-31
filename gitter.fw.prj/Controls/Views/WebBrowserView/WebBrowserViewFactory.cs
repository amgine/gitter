﻿#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

using Resources = gitter.Framework.Properties.Resources;

public class WebBrowserViewFactory : ViewFactoryBase
{
	public static readonly new Guid Guid = new("BF80569F-4544-4B0F-8C5B-213215E053AA");

	public WebBrowserViewFactory()
		: base(Guid, Resources.StrWebBrowser, CommonIcons.WebBrowser, singleton: true)
	{
		this.DefaultViewPosition = ViewPosition.SecondaryDocumentHost;
	}

	/// <inheritdoc/>
	protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		=> new WebBrowserView(Guid, environment);
}
