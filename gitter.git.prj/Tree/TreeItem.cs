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
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public abstract class TreeItem : GitNamedObjectWithLifetime
	{
		#region Data

		private TreeDirectory _parent;
		private FileStatus _status;
		private string _relativePath;
		private StagedStatus _stagedStatus;

		#endregion

		#region Events

		public event EventHandler StatusChanged;

		public event EventHandler StagedStatusChanged;

		#endregion

		#region .ctor

		protected TreeItem(Repository repository, string relativePath,
			TreeDirectory parent, FileStatus status, string name)
			: base(repository, name)
		{
			_parent = parent;
			if(parent != null)
			{
				_stagedStatus = parent._stagedStatus;
			}
			_status = status;
			_relativePath = relativePath;
		}

		protected TreeItem(Repository repository, string relativePath,
			TreeDirectory parent, string name)
			: this(repository, relativePath, parent, FileStatus.Unknown, name)
		{
		}

		#endregion

		public StagedStatus StagedStatus
		{
			get { return _stagedStatus; }
			internal set
			{
				Assert.IsFalse(IsDeleted);

				if(_stagedStatus != value)
				{
					_stagedStatus = value;
					StagedStatusChanged.Raise(this);
				}
			}
		}

		public FileStatus Status
		{
			get { return _status; }
			internal set
			{
				Assert.IsFalse(IsDeleted);

				if(_status != value)
				{
					_status = value;
					StatusChanged.Raise(this);
				}
			}
		}

		public abstract TreeItemType ItemType { get; }

		#region Methods

		public void Stage()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Status.Stage(this);
			StagedStatus = StagedStatus.Staged;
		}

		public void Stage(AddFilesMode mode)
		{
			Verify.State.IsNotDeleted(this);

			Repository.Status.Stage(this, mode);
			StagedStatus = StagedStatus.Staged;
		}

		public void Unstage()
		{
			Verify.State.IsNotDeleted(this);

			Repository.Status.Unstage(this);
			StagedStatus = StagedStatus.Unstaged;
		}

		public IDiffSource GetDiffSource()
		{
			Verify.State.IsNotDeleted(this);

			switch(_stagedStatus)
			{
				case StagedStatus.Staged:
					return Repository.Status.GetDiffSource(true, new[] { RelativePath });
				case StagedStatus.Unstaged:
					return Repository.Status.GetDiffSource(false, new[] { RelativePath });
			}
			return null;
		}

		public void Remove()
		{
			Remove(false);
		}

		public void Remove(bool force)
		{
			Verify.State.IsNotDeleted(this);

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.RemoveFiles.Invoke(
					new RemoveFilesParameters(RelativePath)
					{
						Cached = _stagedStatus == Git.StagedStatus.Staged,
						Force = force,
					});
			}
			Repository.Status.Refresh();
		}

		public void RemoveFromWorkingTree()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdated))
			{
				System.IO.File.Delete(FullPath);
			}
			Repository.Status.Refresh();
		}

		public void Revert()
		{
			Verify.State.IsNotDeleted(this);
			Verify.State.IsTrue((_stagedStatus & StagedStatus.Unstaged) == StagedStatus.Unstaged);

			switch(ItemType)
			{
				case TreeItemType.Tree:
					RevertCore();
					break;
				case TreeItemType.Commit:
				case TreeItemType.Blob:
					switch(Status)
					{
						case FileStatus.Removed:
						case FileStatus.Modified:
							RevertCore();
							break;
						default:
							throw new InvalidOperationException("Inappropriate status.");
					}
					break;
			}
		}

		private void RevertCore()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.CheckoutFiles.Invoke(
					new CheckoutFilesParameters(RelativePath)
					{
						Mode = CheckoutFileMode.IgnoreUnmergedEntries,
					});
			}
			Repository.Status.Refresh();
		}

		#endregion

		public TreeDirectory Parent
		{
			get { return _parent; }
			internal set { _parent = value; }
		}

		public string FullPath
		{
			get
			{
				var sb = new StringBuilder();
				var root = Repository.WorkingDirectory;
				sb.Append(root);
				if(!root.EndsWith(Path.DirectorySeparatorChar) && !root.EndsWith(Path.AltDirectorySeparatorChar))
				{
					sb.Append(Path.DirectorySeparatorChar);
				}
				if(_parent != null)
				{
					var stack = new Stack<string>();
					var p = _parent;
					while(p != null && p.Parent != null)
					{
						if(!string.IsNullOrWhiteSpace(p.Name))
						{
							stack.Push(p.Name);
						}
						p = p.Parent;
					}
					while(stack.Count != 0)
					{
						var name = stack.Pop();
						sb.Append(name);
						sb.Append(Path.DirectorySeparatorChar);
					}
					sb.Append(Name);
				}
				else
				{
					sb.Append(RelativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
				}
				return sb.ToString();
			}
		}

		public string RelativePath
		{
			get { return _relativePath; }
		}

		public override string ToString()
		{
			return _relativePath;
		}
	}
}
