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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	public abstract class BaseRemoteReference : IRemoteReference
	{
		#region Events

		public event EventHandler Deleted;

		private void InvokeDeleted()
			=> Deleted?.Invoke(this, EventArgs.Empty);

		#endregion

		internal BaseRemoteReference(RemoteReferencesCollection refs, string name, Hash hash)
		{
			Verify.Argument.IsNotNull(refs, nameof(refs));
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			References = refs;
			Name = name;
			Hash = hash;
		}

		protected abstract void DeleteCore();

		protected abstract Task DeleteCoreAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		public void Delete()
		{
			DeleteCore();
			MarkAsDeleted();
		}

		public async Task DeleteAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			var task = DeleteCoreAsync(progress, cancellationToken);
			await task;

			if(task.IsCompleted)
			{
				MarkAsDeleted();
			}
		}

		public void MarkAsDeleted()
		{
			if(!IsDeleted)
			{
				IsDeleted = true;
				InvokeDeleted();
			}
		}

		public bool IsDeleted { get; private set; }

		protected RemoteReferencesCollection References { get; }

		public Remote Remote => References.Remote;

		public string Name { get; }

		public string FullName
		{
			get
			{
				switch(ReferenceType)
				{
					case ReferenceType.LocalBranch:
						return GitConstants.LocalBranchPrefix + Name;
					case ReferenceType.Tag:
						return GitConstants.TagPrefix + Name;
					default:
						return Name;
				}
			}
		}

		public Hash Hash { get; }

		public abstract ReferenceType ReferenceType { get; }

		public override string ToString() => Name;
	}
}
