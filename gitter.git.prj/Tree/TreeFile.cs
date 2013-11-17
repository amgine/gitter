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
	using System.Threading;
	using System.Threading.Tasks;

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
			Repository.Accessor.CheckoutFiles.Invoke(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Theirs
				});
		}

		private void UseOurs()
		{
			Repository.Accessor.CheckoutFiles.Invoke(
				new CheckoutFilesParameters(RelativePath)
				{
					Mode = CheckoutFileMode.Ours
				});
		}

		#region mergetool

		private void RunMergeToolCore(MergeTool mergeTool)
		{
			try
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated))
				{
					Repository.Accessor.RunMergeTool.Invoke(
						new RunMergeToolParameters(RelativePath)
						{
							Tool = mergeTool == null ? null : mergeTool.Name,
						});
				}
			}
			finally
			{
				Repository.Status.Refresh();
			}
		}

		private Task RunMergeToolAsyncCore(MergeTool mergeTool, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			if(progress != null)
			{
				progress.Report(new OperationProgress(Resources.StrWaitingMergeTool.AddEllipsis()));
			}
			var blockedNotifications = Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated);
			return Repository.Accessor
				.RunMergeTool.InvokeAsync(
					new RunMergeToolParameters(RelativePath)
					{
						Tool = mergeTool == null ? null : mergeTool.Name,
					},
					progress,
					cancellationToken)
				.ContinueWith(
				t =>
				{
					blockedNotifications.Dispose();
					Repository.Status.Refresh();
				},
				CancellationToken.None,
				TaskContinuationOptions.None,
				TaskScheduler.Default);
		}

		public void RunMergeTool()
		{
			Verify.State.IsFalse(ConflictType == ConflictType.None);

			RunMergeToolCore(null);
		}

		public void RunMergeTool(MergeTool mergeTool)
		{
			Verify.State.IsFalse(ConflictType == ConflictType.None);

			RunMergeToolCore(mergeTool);
		}

		public Task RunMergeToolAsync(IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(ConflictType == ConflictType.None);

			return RunMergeToolAsyncCore(null, progress, cancellationToken);
		}

		public Task RunMergeToolAsync(MergeTool mergeTool, IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.State.IsFalse(ConflictType == ConflictType.None);

			return RunMergeToolAsyncCore(mergeTool, progress, cancellationToken);
		}

		#endregion
	}
}
