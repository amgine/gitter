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
	using System.Linq;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading.Tasks;

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

		public Task DropAsync(IProgress<OperationProgress> progress)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.DropAsync(this, progress);
		}

		public void Drop()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Drop(this);
		}

		public Task PopAsync(bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.PopAsync(this, restoreIndex, progress);
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

		public void Apply()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Apply(this, false);
		}

		public void Apply(bool restoreIndex)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Stash.Apply(this, restoreIndex);
		}

		public Task ApplyAsync(bool restoreIndex, IProgress<OperationProgress> progress)
		{
			Verify.State.IsNotDeleted(this);

			return Repository.Stash.ApplyAsync(this, restoreIndex, progress);
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
