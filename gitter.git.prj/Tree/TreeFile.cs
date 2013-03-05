namespace gitter.Git
{
	using System;
	using System.IO;

	using gitter.Framework;

	using gitter.Git.AccessLayer;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>Represents a file in a directory.</summary>
	public sealed class TreeFile : TreeItem
	{
		#region Data

		private ConflictType _conflictType;
		private long _size;

		#endregion

		#region .ctor

		public TreeFile(Repository repository, string relativePath, TreeDirectory parent, FileStatus status, string name)
			: base(repository, relativePath, parent, status, name)
		{
		}

		public TreeFile(Repository repository, string relativePath, TreeDirectory parent, FileStatus status, string name, long size)
			: base(repository, relativePath, parent, status, name)
		{
			_size = size;
		}

		#endregion

		public override TreeItemType ItemType
		{
			get { return TreeItemType.Blob; }
		}

		public ConflictType ConflictType
		{
			get { return _conflictType; }
			internal set { _conflictType = value; }
		}

		public long Size
		{
			get { return _size; }
		}

		public void ResolveConflict(ConflictResolution resolution)
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				switch(resolution)
				{
					case ConflictResolution.DeleteFile:
						Remove(true);
						break;
					case ConflictResolution.KeepModifiedFile:
						Stage(AddFilesMode.Default);
						break;
					case ConflictResolution.UseOurs:
						UseOurs();
						Stage(AddFilesMode.Default);
						break;
					case ConflictResolution.UseTheirs:
						UseTheirs();
						Stage(AddFilesMode.Default);
						break;
					default:
						throw new ArgumentException(
							"Unknown ConflictResolution value: {0}".UseAsFormat(resolution),
							"resolution");
				}
			}
		}

		private void UseTheirs()
		{
			Repository.Accessor.CheckoutFiles(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Theirs
				});
		}

		private void UseOurs()
		{
			Repository.Accessor.CheckoutFiles(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Ours
				});
		}

		#region mergetool

		private void RunMergeTool(MergeTool mergeTool, IAsyncProgressMonitor monitor)
		{
			try
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated))
				{
					Repository.Accessor.RunMergeTool(
						new RunMergeToolParameters(RelativePath)
						{
							Monitor = monitor,
							Tool = mergeTool == null ? null : mergeTool.Name,
						});
				}
			}
			finally
			{
				Repository.Status.Refresh();
			}
		}

		public void RunMergeTool()
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			RunMergeTool(null, null);
		}

		public void RunMergeTool(MergeTool mergeTool)
		{
			Verify.Argument.IsNotNull(mergeTool, "mergeTool");
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			RunMergeTool(mergeTool, null);
		}

		public IAsyncAction RunMergeToolAsync()
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			return AsyncAction.Create(this,
				(file, mon) =>
				{
					file.RunMergeTool(null, mon);
				},
				Resources.StrRunningMergeTool,
				Resources.StrWaitingMergeTool.AddEllipsis(),
				true);
		}

		public IAsyncAction RunMergeToolAsync(MergeTool mergeTool)
		{
			Verify.State.IsFalse(ConflictType == Git.ConflictType.None);

			return AsyncAction.Create(new
				{
					File = this,
					Tool = mergeTool,
				},
				(data, mon) =>
				{
					data.File.RunMergeTool(data.Tool, mon);
				},
				Resources.StrRunningMergeTool,
				Resources.StrWaitingMergeTool.AddEllipsis(),
				true);
		}

		#endregion
	}
}
