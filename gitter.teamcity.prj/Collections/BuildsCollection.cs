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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using gitter.Framework;

public sealed class BuildsCollection : TeamCityObjectsCache<Build>
{
	const string QUERY = @"builds/?locator=";

	internal BuildsCollection(TeamCityServiceContext context)
		: base(context)
	{
	}

	private static string GetUrl(BuildLocator locator)
	{
		var url = new StringBuilder(QUERY);
		url.Append(locator.ToString());
		url.Append("&fields=");
		url.Append("count");
		url.Append(',');
		url.Append("build(");
		url.Append("id,");
		url.Append("number,");
		url.Append("buildTypeId,");
		url.Append("branchName,");
		url.Append("defaultBranch,");
		url.Append("queuedDate,");
		url.Append("startDate,");
		url.Append("finishDate,");
		url.Append("history,");
		url.Append("composite,");
		url.Append("statusText,");
		url.Append("status,");
		url.Append("state,");
		url.Append("failedToStart,");
		url.Append("personal,");
		url.Append("detachedFromAgent,");
		url.Append("finishOnAgentDate,");
		url.Append("connected,");
		url.Append("webUrl");
		url.Append(')');
		return url.ToString();
	}

	public async Task<Build[]> QueryAsync(BuildLocator locator, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(locator);

		var xml = await Context
			.GetXmlAsync(GetUrl(locator), cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var root = xml["builds"];
		if(root is null) return Preallocated<Build>.EmptyArray;
		var result = new Build[TeamCityUtility.LoadInt(root.Attributes["count"])];
		int id = 0;
		foreach(XmlElement node in root.ChildNodes)
		{
			result[id++] = Lookup(node);
		}
		return result;
	}

	public async Task UpdateCacheAsync(BuildLocator locator, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(locator);

		var xml = await Context
			.GetXmlAsync(GetUrl(locator), cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		var builds = xml["builds"];
		if(builds is not null)
		{
			foreach(XmlElement node in builds.ChildNodes)
			{
				Lookup(node);
			}
		}
	}

	protected override Build Create(string id)
		=> new(Context, id);

	protected override Build Create(XmlNode node)
		=> new(Context, node);
}
