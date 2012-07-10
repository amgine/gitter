namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	public abstract class TreeItem : GitLifeTimeNamedObject
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
				if(_status != value)
				{
					_status = value;
					StatusChanged.Raise(this);
				}
			}
		}

		public abstract TreeItemType Type { get; }

		#region Methods

		public void Stage()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat(GetType().Name));
			}

			#endregion

			Repository.Status.Stage(this);
			StagedStatus = StagedStatus.Staged;
		}

		public void Stage(AddFilesMode mode)
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat(GetType().Name));
			}

			#endregion

			Repository.Status.Stage(this, mode);
			StagedStatus = StagedStatus.Staged;
		}

		public void Unstage()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat(GetType().Name));
			}

			#endregion

			Repository.Status.Unstage(this);
			StagedStatus = StagedStatus.Unstaged;
		}

		public IDiffSource GetDiffSource()
		{
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
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat(GetType().Name));
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.RemoveFiles(
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
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(
					Resources.ExcObjectIsDeleted.UseAsFormat(GetType().Name));
			}
			if((_stagedStatus & StagedStatus.Unstaged) != StagedStatus.Unstaged)
			{
				throw new InvalidOperationException();
			}

			#endregion

			switch(Type)
			{
				case TreeItemType.Tree:
					RevertCore();
					break;
				case TreeItemType.Submodule:
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
				Repository.Accessor.CheckoutFiles(
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
				}
				sb.Append(Name);
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
