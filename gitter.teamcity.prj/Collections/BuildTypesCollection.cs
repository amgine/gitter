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
using System.Xml;

public sealed class BuildTypesCollection : NamedTeamCityObjectsCache<BuildType>
{
	internal BuildTypesCollection(TeamCityServiceContext context)
		: base(context)
	{
	}

	protected override BuildType Create(string id, string name)
		=> new(Context, id, name);

	protected override BuildType Create(string id)
		=> new(Context, id);

	protected override BuildType Create(XmlNode node)
		=> new(Context, node);

	private void UpdateCache(XmlDocument xml)
	{
		var buildTypes = xml["buildTypes"];
		if(buildTypes is not null)
		{
			foreach(XmlElement node in buildTypes)
			{
				Lookup(node);
			}
		}
	}

	public async Task UpdateCacheAsync(CancellationToken cancellationToken = default)
	{
		var xml = await Context
			.GetXmlAsync("buildTypes", cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		UpdateCache(xml);
	}

	public async Task UpdateCacheAsync(ProjectLocator projectLocator, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(projectLocator);
		var pl = projectLocator.ToString();
		Verify.Argument.IsNeitherNullNorWhitespace(pl, nameof(projectLocator));

		var xml = await Context
			.GetXmlAsync("projects/" + pl + "/buildTypes", cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		UpdateCache(xml);
	}
}
