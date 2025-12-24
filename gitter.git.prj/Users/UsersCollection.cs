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

namespace gitter.Git;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using gitter.Framework;

using gitter.Git.AccessLayer;

/// <summary>Repository committers collection.</summary>
public sealed class UsersCollection : GitObjectsCollection<UsersCollection.Key, User, UserEventArgs>
{
	public readonly record struct Key(string Name, string Email);

	sealed class Updater(UsersCollection collection)
		: CacheUpdater<Key, User, UserData>(collection._dictionary, collection.SyncRoot)
	{
		protected override User CreateObject(UserData data)
			=> ObjectFactories.CreateUser(collection.Repository, data);

		protected override void UpdateObject(User @object, UserData data)
			=> ObjectFactories.UpdateUser(@object, data);

		protected override Key GetKey(User @object)
			=> new(@object.Name, @object.Email);

		protected override Key GetKey(UserData data)
			=> new(data.Name, data.Email);

		protected override void OnObjectAdded(User @object)
			=> collection.InvokeObjectAdded(@object);

		protected override void OnObjectRemoved(User @object)
			=> collection.InvokeObjectRemoved(@object);
	}

	private readonly Updater _updater;

	/// <summary>Create <see cref="UsersCollection"/>.</summary>
	/// <param name="repository">Host <see cref="Repository"/>.</param>
	internal UsersCollection(Repository repository)
		: base(repository)
	{
		_updater = new(this);
	}

	protected override Key GetKey(User @object) => new(@object.Name, @object.Email);

	protected override UserEventArgs CreateEventArgs(User item) => new(item);

	public User this[string name, string email]
	{
		get { lock(SyncRoot) return ObjectStorage[new(name, email)]; }
	}

	public bool TryGetUser(string name, string email,
		[MaybeNullWhen(returnValue: false)] out User user)
	{
		lock(SyncRoot)
		{
			return ObjectStorage.TryGetValue(new(name, email), out user);
		}
	}

	public User? TryGetUser(string name, string email)
	{
		lock(SyncRoot)
		{
			if(ObjectStorage.TryGetValue(new(name, email), out var user))
			{
				return user;
			}
		}
		return null;
	}

	internal User GetOrCreateUser(string name, string email)
	{
		var key = new Key(name, email);
		lock(SyncRoot)
		{
			if(!_dictionary.TryGetValue(key, out var user))
			{
				_dictionary.Add(key, user = new(Repository, name, email, 0));
				InvokeObjectAdded(user);
			}
			return user;
		}
	}

	public void Refresh()
	{
		var users = Repository.Accessor.QueryUsers.Invoke(
			new QueryUsersRequest());
		_updater.Update(users);
	}

	public async Task RefreshAsync(CancellationToken cancellationToken = default)
	{
		var users = await Repository.Accessor.QueryUsers
			.InvokeAsync(new QueryUsersRequest(), cancellationToken: cancellationToken)
			.ConfigureAwait(continueOnCapturedContext: false);
		_updater.Update(users);
	}
}
