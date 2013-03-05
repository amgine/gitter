namespace gitter.Git
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Globalization;

	using gitter.Framework;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Represents stashed state.</summary>
	public sealed class StashedState : GitNamedObjectWithLifetime, IRevisionPointer
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
			Verify.Argument.IsNotNull(revision, "revision");
			Verify.Argument.IsNotNegative(index, "index");

			_revision = revision;
		}

		#endregion

		#region Methods

		public IAsyncAction DropAsync()
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.DropAsync(this);
		}

		public void Drop()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Drop(this);
		}

		public IAsyncAction PopAsync(bool restoreIndex)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.PopAsync(this, restoreIndex);
		}

		public void Pop(bool restoreIndex)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Pop(this, restoreIndex);
		}

		public void Pop()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Pop(this, false);
		}

		public IAsyncAction ApplyAsync(bool restoreIndex)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.ApplyAsync(this, restoreIndex);
		}

		public void Apply(bool restoreIndex)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Apply(this, restoreIndex);
		}

		public void Apply()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Apply(this, false);
		}

		public Branch ToBranch(string name)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.ToBranch(this, name);
		}

		public IRevisionDiffSource GetDiffSource()
		{
			Verify.State.IsNotDeleted(this);

			return new StashedChangesDiffSource(this);
		}

		public IRevisionDiffSource GetDiffSource(IEnumerable<string> paths)
		{
			Verify.State.IsNotDeleted(this);

			if(paths == null)
			{
				return new StashedChangesDiffSource(this);
			}
			else
			{
				return new StashedChangesDiffSource(this, paths.ToList());
			}
		}

		#endregion

		#region Properties

		/// <summary>Returns stash index.</summary>
		/// <value>Stash index.</value>
		public int Index
		{
			get { return _index; }
			internal set
			{
				Verify.Argument.IsNotNegative(value, "value");

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
			get { return ReferenceType.Stash; }
		}

		string IRevisionPointer.Pointer
		{
			get { return GitConstants.StashFullName + "@{" + _index.ToString(CultureInfo.InvariantCulture) + "}"; }
		}

		string IRevisionPointer.FullName
		{
			get { return GitConstants.StashFullName + "@{" + _index.ToString(CultureInfo.InvariantCulture) + "}"; }
		}

		public Revision Dereference()
		{
			return _revision;
		}

		#endregion
	}
}
