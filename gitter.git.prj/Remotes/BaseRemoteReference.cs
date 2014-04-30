#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2014  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	public abstract class BaseRemoteReference : IRemoteReference
	{
		#region Events

		public event EventHandler Deleted;

		private void InvokeDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Data

		private readonly RemoteReferencesCollection _refs;
		private readonly string _name;
		private readonly Hash _hash;

		private bool _deleted;

		#endregion

		internal BaseRemoteReference(RemoteReferencesCollection refs, string name, Hash hash)
		{
			Verify.Argument.IsNotNull(refs, "refs");
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_refs = refs;
			_name = name;
			_hash = hash;
		}

		protected abstract void DeleteCore();

		protected abstract Task DeleteCoreAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		public void Delete()
		{
			DeleteCore();
			MarkAsDeleted();
		}

		public Task DeleteAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			return DeleteCoreAsync(progress, cancellationToken)
				.ContinueWith(
				t =>
				{
					if(t.IsCompleted)
					{
						MarkAsDeleted();
					}
					TaskUtility.PropagateFaultedStates(t);
				},
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}

		public void MarkAsDeleted()
		{
			if(!_deleted)
			{
				_deleted = true;
				InvokeDeleted();
			}
		}

		public bool IsDeleted
		{
			get { return _deleted; }
		}

		protected RemoteReferencesCollection References
		{
			get { return _refs; }
		}

		public Remote Remote
		{
			get { return _refs.Remote; }
		}

		public string Name
		{
			get { return _name; }
		}

		public string FullName
		{
			get
			{
				switch(ReferenceType)
				{
					case ReferenceType.LocalBranch:
						return GitConstants.LocalBranchPrefix + _name;
					case ReferenceType.Tag:
						return GitConstants.TagPrefix + _name;
					default:
						return _name;
				}
			}
		}

		public Hash Hash
		{
			get { return _hash; }
		}

		public abstract ReferenceType ReferenceType { get; }

		public override string ToString()
		{
			return _name;
		}
	}
}
