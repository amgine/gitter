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

namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	/// <summary>Repository committers collection.</summary>
	public sealed class UsersCollection : GitObjectsCollection<User, UserEventArgs>
	{
		#region .ctor

		/// <summary>Create <see cref="UsersCollection"/>.</summary>
		/// <param name="repository">Host <see cref="Repository"/>.</param>
		internal UsersCollection(Repository repository)
			: base(repository)
		{
		}

		#endregion

		protected override UserEventArgs CreateEventArgs(User item)
		{
			return new UserEventArgs(item);
		}

		public User this[string name, string email]
		{
			get { lock(SyncRoot) return ObjectStorage[name + "\n" + email]; }
		}

		public bool TryGetUser(string name, string email, out User user)
		{
			lock(SyncRoot)
			{
				return ObjectStorage.TryGetValue(name + "\n" + email, out user);
			}
		}

		public User TryGetUser(string name, string email)
		{
			User user;
			lock(SyncRoot)
			{
				if(ObjectStorage.TryGetValue(name + "\n" + email, out user))
				{
					return user;
				}
			}
			return null;
		}

		internal User GetOrCreateUser(string name, string email)
		{
			User user;
			lock(SyncRoot)
			{
				if(!ObjectStorage.TryGetValue(name + "\n" + email, out user))
				{
					user = new User(Repository, name, email, 0);
					AddObject(user);
				}
			}
			return user;
		}

		public void Refresh()
		{
			var users = Repository.Accessor.QueryUsers.Invoke(
				new QueryUsersParameters());
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<User, UserData>(
					ObjectStorage,
					null,
					null,
					users,
					userData => ObjectFactories.CreateUser(Repository, userData),
					ObjectFactories.UpdateUser,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}
	}
}
