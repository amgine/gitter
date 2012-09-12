namespace gitter.Git
{
	using System;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Base class for all repository-related objects.</summary>
	public abstract class GitObject
	{
		private readonly Repository _repository;

		/// <summary>Create <see cref="GitObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		protected GitObject(Repository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;
		}

		/// <summary>Create <see cref="GitObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="allowNullRepository"><paramref name="repository"/> can be null.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="repository"/> == <c>null</c>.</exception>
		protected GitObject(Repository repository, bool allowNullRepository)
		{
			if(!allowNullRepository)
			{
				Verify.Argument.IsNotNull(repository, "repository");
			}
			_repository = repository;
		}

		/// <summary>Host repository.</summary>
		public Repository Repository
		{
			get { return _repository; }
		}
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
		/// Marks this object as delected, invokes <see cref="ILifetimeObject.Deleted"/>
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
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name = name;
		}

		/// <summary>Create <see cref="GitNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitNamedObject(string name)
			: base(null, true)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name = name;
		}

		/// <summary>Object name.</summary>
		public string Name
		{
			get { return _name; }
			set
			{
				Verify.Argument.IsNeitherNullNorWhitespace(value, "value");

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
		public override string ToString()
		{
			return _name;
		}
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
		public string Name
		{
			get { return GetName(); }
		}

		/// <summary>Returns a <see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</summary>
		/// <returns><see cref="T:System.String"/> representation of this <see cref="GitNamedObject"/>.</returns>
		public override string ToString()
		{
			return GetName();
		}
	}

	/// <summary>git named object with lifetime control.</summary>
	public abstract class GitLifeTimeNamedObject : GitNamedObject
	{
		/// <summary>This object has been deleted.</summary>
		public event EventHandler Deleted;

		/// <summary>This object has been revived.</summary>
		public event EventHandler Revived;

		private bool _deleted;

		/// <summary>Create <see cref="GitLifeTimeNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitLifeTimeNamedObject(Repository repository, string name)
			: base(repository, name)
		{
		}

		/// <summary>Create <see cref="GitLifeTimeNamedObject"/>.</summary>
		/// <param name="name">Object name.</param>
		protected GitLifeTimeNamedObject(string name)
			: base(name)
		{
		}

		/// <summary>Marks this object as delected, invokes <see cref="Deleted"/> and makes all methods of this object fail.</summary>
		internal void MarkAsDeleted()
		{
			if(!_deleted)
			{
				_deleted = true;
				OnDeleted();
				var handler = Deleted;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		/// <summary>Makes object alive again.</summary>
		internal void Revive()
		{
			if(_deleted)
			{
				_deleted = false;
				OnRevived();
				var handler = Revived;
				if(handler != null) handler(this, EventArgs.Empty);
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
		public bool IsDeleted
		{
			get { return _deleted; }
		}
	}

	/// <summary>git named object with lifetime control.</summary>
	public abstract class GitLifeTimeDynamicNamedObject : GitDynamicNamedObject
	{
		/// <summary>This object has been deleted.</summary>
		public event EventHandler Deleted;

		/// <summary>This object has been revived.</summary>
		public event EventHandler Revived;

		private bool _isDeleted;

		/// <summary>Create <see cref="GitLifeTimeNamedObject"/>.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Object name.</param>
		protected GitLifeTimeDynamicNamedObject(Repository repository)
			: base(repository)
		{
		}

		/// <summary>Marks this object as delected, invokes <see cref="Deleted"/> and makes all methods of this object fail.</summary>
		internal void MarkAsDeleted()
		{
			if(!_isDeleted)
			{
				_isDeleted = true;
				OnDeleted();
				var handler = Deleted;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		/// <summary>Makes object alive again.</summary>
		internal void Revive()
		{
			if(_isDeleted)
			{
				_isDeleted = false;
				OnRevived();
				var handler = Revived;
				if(handler != null) handler(this, EventArgs.Empty);
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
		public bool IsDeleted
		{
			get { return _isDeleted; }
		}
	}
}
