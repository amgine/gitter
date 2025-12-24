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

namespace gitter.TeamCity;

using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class BuildTypeBuildsCollection : CacheSegment<Build>
{
	private readonly BuildType _buildType;

	internal BuildTypeBuildsCollection(BuildType buildType, BuildsCollection builds)
		: base(builds)
	{
		Verify.Argument.IsNotNull(buildType);

		_buildType = buildType;
	}

	protected override bool IsIncluded(Build item)
		=> item.BuildType == _buildType;

	public override Task RefreshAsync(CancellationToken cancellationToken = default)
	{
		var locator = new BuildLocator() { BuildType = _buildType.CreateLocator() };
		return Context.Builds.UpdateCacheAsync(locator, cancellationToken);
	}
}
