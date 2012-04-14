namespace gitter.Git
{
	using System;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Represents stashed state.</summary>
	public sealed class StashedState : GitLifeTimeNamedObject, IRevisionPointer
	{
		#region Events

		public event EventHandler IndexChanged;

		private void InvokeIndexChanged()
		{
			var handler = IndexChanged;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private int _index;
		private readonly Revision _revision;

		#endregion

		#region .ctor

		internal StashedState(Repository repository, int index, Revision revision)
			: base(repository, index.ToString().SurroundWith(GitConstants.StashName + "@{", "}"))
		{
			if(revision == null) throw new ArgumentNullException("revision");
			_revision = revision;
		}

		#endregion

		#region Methods

		public IAsyncAction DropAsync()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			return Repository.Stash.DropAsync(this);
		}

		public void Drop()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			Repository.Stash.Drop(this);
		}

		public IAsyncAction PopAsync(bool restoreIndex)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			return Repository.Stash.PopAsync(this, restoreIndex);
		}

		public void Pop(bool restoreIndex)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			Repository.Stash.Pop(this, restoreIndex);
		}

		public void Pop()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			Repository.Stash.Pop(this, false);
		}

		public IAsyncAction ApplyAsync(bool restoreIndex)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			return Repository.Stash.ApplyAsync(this, restoreIndex);
		}

		public void Apply(bool restoreIndex)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			Repository.Stash.Apply(this, restoreIndex);
		}

		public void Apply()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			Repository.Stash.Apply(this, false);
		}

		public Branch ToBranch(string name)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "StashedState"));
			}

			#endregion

			return Repository.Stash.ToBranch(this, name);
		}

		#endregion

		#region Properties

		public int Index
		{
			get { return _index; }
			internal set
			{
				if(_index != value)
				{
					_index = value;
					InvokeIndexChanged();
				}
			}
		}

		public Revision Revision
		{
			get { return _revision; }
		}

		#endregion

		#region IRevisionPointer

		ReferenceType IRevisionPointer.Type
		{
			get { return ReferenceType.Revision; }
		}

		string IRevisionPointer.Pointer
		{
			get { return GitConstants.StashFullName + "@{" + _index.ToString() + "}"; }
		}

		string IRevisionPointer.FullName
		{
			get { return GitConstants.StashFullName + "@{" + _index.ToString() + "}"; }
		}

		public Revision Dereference()
		{
			return _revision;
		}

		#endregion
	}
}
