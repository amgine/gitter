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

using gitter.Framework;

using Resources = gitter.Framework.Properties.Resources;

public sealed class LogViewFactory : ViewFactoryBase
{
	public static readonly new Guid Guid = new("216F243F-0E79-4739-A88F-C2342E5975B6");

	/// <summary>Initializes a new instance of the <see cref="LogViewFactory"/> class.</summary>
	public LogViewFactory()
		: base(Guid, Resources.StrLog, new ScaledImageProvider(CachedResources.ScaledBitmaps, @"log.view"), singleton: true)
	{
		DefaultViewPosition = ViewPosition.Float;
	}

	/// <inheritdoc/>
	protected override ViewBase CreateViewCore(IWorkingEnvironment environment)
		=> new LogView(environment);
}
