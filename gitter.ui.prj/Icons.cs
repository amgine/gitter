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

namespace gitter;

using gitter.Framework;

static class Icons
{
	static IImageProvider CreateProvider(string name)
		=> new ScaledImageProvider(CachedResources.ScaledBitmaps, name);

	public static readonly IImageProvider Repository         = CreateProvider(@"repository");
	public static readonly IImageProvider RepositoryOpen     = CreateProvider(@"repository.open");
	public static readonly IImageProvider RepositoryAdd      = CreateProvider(@"repository.add");
	public static readonly IImageProvider RepositoryClone    = CreateProvider(@"repository.clone");
	public static readonly IImageProvider RepositoryInit     = CreateProvider(@"repository.init");
	public static readonly IImageProvider RepositoryScan     = CreateProvider(@"repository.scan");
	public static readonly IImageProvider RepositoryRemove   = CreateProvider(@"repository.remove");
	public static readonly IImageProvider RepositoryExplorer = CreateProvider(@"repository.explorer");
	public static readonly IImageProvider StartPage          = CreateProvider(@"start.page");
}
