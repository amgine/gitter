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

namespace gitter.Redmine;

#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public sealed class UsersCollection : NamedRedmineObjectsCache<User>
{
	internal UsersCollection(RedmineServiceContext context)
		: base(context)
	{
	}

	protected override User Create(int id, string name)
		=> new(Context, id, name);

	protected override User Create(XmlNode node)
		=> new(Context, node);

	public Task<List<User>> FetchAsync(CancellationToken cancellationToken = default)
	{
		const string url = "users.xml";
		return FetchItemsFromAllPagesAsync(url, cancellationToken);
	}

	public Task<User> FetchCurrentAsync(CancellationToken cancellationToken = default)
	{
		const string url = "users/current.xml";
		return FetchSingleItemAsync(url, cancellationToken);
	}
}
