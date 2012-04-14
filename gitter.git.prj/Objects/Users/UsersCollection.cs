namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	/// <summary>Repository committers collection.</summary>
	public sealed class UsersCollection : GitObjectCollection<User, UserEventArgs>
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
			var users = Repository.Accessor.QueryUsers(
				new QueryUsersParameters());
			lock(SyncRoot)
			{
				CacheUpdater.UpdateObjectDictionary<User, UserData>(
					Repository,
					ObjectStorage,
					null,
					null,
					users,
					InvokeObjectAdded,
					InvokeObjectRemoved,
					true);
			}
		}
	}
}
