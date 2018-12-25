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

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Base class for all repository-related objects.</summary>
	public abstract class GitObject
	{
		/// <summary>Create <see cref="GitObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		protected GitObject(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, nameof(repository));

			Repository = repository;
		}

		/// <summary>Create <see cref="GitObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="allowNullRepository"><paramref name="repository"/> can be null.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		protected GitObject(Repository repository, bool allowNullRepository)
		{
			if(!allowNullRepository)
			{
				Verify.Argument.IsNotNull(repository, nameof(repository));
			}
			Repository = repository;
		}

		/// <summary>Host repository.</summary>
		public Repository Repository { get; }
	}

	public interface ILifetimeObject
	{
		/// <summary>This object has been deleted.</summary>
		event EventHandler Deleted;

		/// <summary>This object has been revived.</summary>
		event EventHandler Revived;

		/// <summary>Checks if object is deleted.</summary>
		bool IsDeleted { get; }

		/// <summary>Checks if object is alive.</summary>
		bool IsAlive { get; }
	}

	internal interface ILifetimeObjectControlService
	{
		/// <summary>
		/// Marks this object as deleted, invokes <see cref="ILifetimeObject.Deleted"/>
		/// and makes all methods of this object fail.
		/// </summary>
		void MarkAsDeleted();

		/// <summary>Makes object alive again.</summary>
		void Revive();
	}

	/// <summary>git object with a name.</summary>
	public abstract class GitNamedObject : GitObject, INamedObject
	{
		private string _name;

		/// <summary>Create <see cref="GitNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitNamedObject(Repository repository, string name)
			: base(repository)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			_name = name;
		}

		/// <summary>Create <see cref="GitNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitNamedObject(string name)
			: base(null, true)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			_name = name;
		}

		/// <summary>Object name.</summary>
		public string Name
		{
			get { return _name; }
			set
			{
				Verify.Argument.IsNeitherNullNorWhitespace(value, nameof(value));

				RenameCore(value);
				string oldName = _name;
				_name = value;
				AfterRename(oldName);
			}
		}

		/// <summary>Override this if object supports renaming.</summary>
		/// <param name="newName">New object's name.</param>
		/// <exception cref="T:System.NotSupportedException">Object does not support renaming.</exception>
		protected virtual void RenameCore(string newName)
		{
			throw new NotSupportedException();
		}

		/// <summary>Override this if object supports renaming.</summary>
		/// <param name="oldName">Old object's name.</param>
		protected virtual void AfterRename(string oldName)
		{
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</returns>
		public override string ToString() => _name;
	}

	/// <summary>git object with a dynamic name.</summary>
	public abstract class GitDynamicNamedObject : GitObject, INamedObject
	{
		/// <summary>Create <see cref="GitNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitDynamicNamedObject(Repository repository)
			: base(repository)
		{
		}

		/// <summary>Create <see cref="GitNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		protected GitDynamicNamedObject()
			: base(null, true)
		{
		}

		protected abstract string GetName();

		/// <summary>Object name.</summary>
		public string Name => GetName();

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</returns>
		public override string ToString() => Name;
	}

	/// <summary>git named object with lifetime control.</summary>
	public abstract class GitNamedObjectWithLifetime : GitNamedObject
	{
		/// <summary>This object has been deleted.</summary>
		public event EventHandler Deleted;

		/// <summary>This object has been revived.</summary>
		public event EventHandler Revived;

		/// <summary>Create <see cref="GitNamedObjectWithLifetime"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitNamedObjectWithLifetime(Repository repository, string name)
			: base(repository, name)
		{
		}

		/// <summary>Create <see cref="GitNamedObjectWithLifetime"/>.</summary>
		/// <param name="name">Object name.</param>
		protected GitNamedObjectWithLifetime(string name)
			: base(name)
		{
		}

		/// <summary>Marks this object as deleted, invokes <see cref="Deleted"/> and makes all methods of this object fail.</summary>
		internal void MarkAsDeleted()
		{
			if(!IsDeleted)
			{
				IsDeleted = true;
				OnDeleted();
				Deleted?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>Makes object alive again.</summary>
		internal void Revive()
		{
			if(IsDeleted)
			{
				IsDeleted = false;
				OnRevived();
				Revived?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>Called after marking as deleted.</summary>
		protected virtual void OnDeleted()
		{
		}

		/// <summary>Called after reviving.</summary>
		protected virtual void OnRevived()
		{
		}

		/// <summary>Checks if object is deleted.</summary>
		public bool IsDeleted { get; private set; }
	}

	/// <summary>git named object with lifetime control.</summary>
	public abstract class GitLifeTimeDynamicNamedObject : GitDynamicNamedObject
	{
		/// <summary>This object has been deleted.</summary>
		public event EventHandler Deleted;

		/// <summary>This object has been revived.</summary>
		public event EventHandler Revived;

		/// <summary>Create <see cref="GitNamedObjectWithLifetime"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitLifeTimeDynamicNamedObject(Repository repository)
			: base(repository)
		{
		}

		/// <summary>Marks this object as deleted, invokes <see cref="Deleted"/> and makes all methods of this object fail.</summary>
		internal void MarkAsDeleted()
		{
			if(!IsDeleted)
			{
				IsDeleted = true;
				OnDeleted();
				Deleted?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>Makes object alive again.</summary>
		internal void Revive()
		{
			if(IsDeleted)
			{
				IsDeleted = false;
				OnRevived();
				Revived?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>Called after marking as deleted.</summary>
		protected virtual void OnDeleted()
		{
		}

		/// <summary>Called after reviving.</summary>
		protected virtual void OnRevived()
		{
		}

		/// <summary>Checks if object is deleted.</summary>
		public bool IsDeleted { get; private set; }
	}
}
