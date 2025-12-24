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

namespace gitter.Git;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using gitter.Framework;

using gitter.Git.AccessLayer;

using Resources = gitter.Git.Properties.Resources;

/// <summary>Working directory status.</summary>
public sealed class Status : GitObject
{
	#region Events

	public event EventHandler? Changed;

	public event EventHandler<TreeFileEventArgs>? NewStagedFile;
	public event EventHandler<TreeDirectoryEventArgs>? NewStagedFolder;

	public event EventHandler<TreeFileEventArgs>? RemovedStagedFile;
	public event EventHandler<TreeDirectoryEventArgs>? RemovedStagedFolder;

	public event EventHandler<TreeFileEventArgs>? NewUnstagedFile;
	public event EventHandler<TreeDirectoryEventArgs>? NewUnstagedFolder;

	public event EventHandler<TreeFileEventArgs>? RemovedUnstagedFile;
	public event EventHandler<TreeDirectoryEventArgs>? RemovedUnstagedFolder;

	public event EventHandler<CommitResultEventArgs>? Committed;

	private void InvokeChanged()
	{
		Changed?.Invoke(this, EventArgs.Empty);
	}

	private void InvokeNewFile(TreeFile file)
	{
		var handler = file.StagedStatus == StagedStatus.Staged ? NewStagedFile : NewUnstagedFile;
		handler?.Invoke(this, new TreeFileEventArgs(file));
	}

	private void InvokeAddedDirectory(TreeDirectory folder)
	{
		var handler = folder.StagedStatus == StagedStatus.Staged ? NewStagedFolder : NewUnstagedFolder;
		handler?.Invoke(this, new TreeDirectoryEventArgs(folder));
	}

	private void InvokeRemovedFile(TreeFile file)
	{
		file.MarkAsDeleted();
		var handler = file.StagedStatus == StagedStatus.Staged ? RemovedStagedFile : RemovedUnstagedFile;
		handler?.Invoke(this, new TreeFileEventArgs(file));
	}

	private void InvokeRemovedDirectory(TreeDirectory folder)
	{
		foreach(var subFolder in folder.Directories)
		{
			InvokeRemovedDirectory(subFolder);
		}
		folder.MarkAsDeleted();
		var handler = folder.StagedStatus == StagedStatus.Staged ? RemovedStagedFolder : RemovedUnstagedFolder;
		handler?.Invoke(this, new TreeDirectoryEventArgs(folder));
		foreach(var file in folder.Files)
		{
			InvokeRemovedFile(file);
		}
	}

	private void OnCommitted(CommitResult commitResult)
		=> Committed?.Invoke(this, new(commitResult));

	#endregion

	#region Data

	private readonly Dictionary<string, TreeFile> _unstagedLookup = [];
	private readonly Dictionary<string, TreeFile> _stagedLookup   = [];

	private readonly TreeDirectory _unstagedRoot;
	private readonly TreeDirectory _stagedRoot;

	private int _unstagedUntrackedCount;
	private int _unstagedRemovedCount;
	private int _unstagedModifiedCount;
	private int _unmergedCount;
	private int _stagedAddedCount;
	private int _stagedModifiedCount;
	private int _stagedRemovedCount;

	#endregion

	/// <summary>Create <see cref="Status"/> object.</summary>
	/// <param name="repository">Related repository.</param>
	internal Status(Repository repository)
		: base(repository)
	{
		var strRoot = Repository.WorkingDirectory;
		if(strRoot.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
		{
			strRoot = strRoot.Substring(0, strRoot.Length - 1);
		}
		_unstagedRoot = new TreeDirectory(Repository, string.Empty, null, strRoot)
			{
				StagedStatus = StagedStatus.Unstaged
			};
		_stagedRoot = new TreeDirectory(Repository, string.Empty, null, strRoot)
			{
				StagedStatus = StagedStatus.Staged
			};
	}

	#region Properties

	public IReadOnlyCollection<TreeFile> StagedFiles => _stagedLookup.Values;

	public TreeDirectory StagedRoot => _stagedRoot;

	public IReadOnlyCollection<TreeFile> UnstagedFiles => _unstagedLookup.Values;

	public TreeDirectory UnstagedRoot => _unstagedRoot;

	public int UnstagedUntrackedCount => _unstagedUntrackedCount;

	public int UnstagedModifiedCount => _unstagedModifiedCount;

	public int UnstagedRemovedCount => _unstagedRemovedCount;

	public int UnmergedCount => _unmergedCount;

	public int StagedAddedCount => _stagedAddedCount;

	public int StagedModifiedCount => _stagedModifiedCount;

	public int StagedRemovedCount => _stagedRemovedCount;

	/// <summary>Object used for cross-thread synchronization.</summary>
	public LockType SyncRoot { get; } = new();

	#endregion

	private static TreeDirectoryData BreakIntoTree(IReadOnlyCollection<TreeFileData> files, StagedStatus stagedStatus)
	{
		Assert.IsNotNull(files);

		static TreeDirectoryData CreateDirectory(TreeDirectoryData root, string path,
			Dictionary<string, TreeDirectoryData> cache, StagedStatus stagedStatus)
		{
			var sindex = path.LastIndexOf('/');
			TreeDirectoryData? dir, parent;
			if(sindex < 0)
			{
				dir = new TreeDirectoryData(path, path, root, FileStatus.Cached, stagedStatus);
				parent = root;
			}
			else
			{
				#if NET9_0_OR_GREATER
				var lookup = cache.GetAlternateLookup<ReadOnlySpan<char>>();
				if(!lookup.TryGetValue(path.AsSpan(0, sindex), out parent))
				{
					var parentPath = path.Substring(0, sindex);
					parent = CreateDirectory(root, parentPath, cache, stagedStatus);
				}
				#else
				var parentPath = path.Substring(0, sindex);
				if(!cache.TryGetValue(parentPath, out parent))
				{
					parent = CreateDirectory(root, parentPath, cache, stagedStatus);
				}
				#endif
				var shortName = path.Substring(sindex + 1);
				dir = new TreeDirectoryData(path, shortName, parent, FileStatus.Cached, stagedStatus);
			}
			cache.Add(path, dir);
			parent.AddDirectory(dir);
			return dir;
		}

		var root = TreeDirectoryData.CreateRoot(FileStatus.Cached, stagedStatus);
		if(files.Count == 0) return root;
		var dirs = new Dictionary<string, TreeDirectoryData>();
		#if NET9_0_OR_GREATER
		var lookup = dirs.GetAlternateLookup<ReadOnlySpan<char>>();
		#endif

		foreach(var tfinfo in files)
		{
			var sindex = tfinfo.Name.LastIndexOf('/');
			if(sindex < 0)
			{
				tfinfo.ShortName = tfinfo.Name;
				root.AddFile(tfinfo);
			}
			else
			{
				tfinfo.ShortName = tfinfo.Name.Substring(sindex + 1);
				#if NET9_0_OR_GREATER
				if(!lookup.TryGetValue(tfinfo.Name.AsSpan(0, sindex), out var dir))
				{
					var path = tfinfo.Name.Substring(0, sindex);
					dir = CreateDirectory(root, path, dirs, stagedStatus);
				}
				#else
				var path = tfinfo.Name.Substring(0, sindex);
				if(!dirs.TryGetValue(path, out var dir))
				{
					dir = CreateDirectory(root, path, dirs, stagedStatus);
				}
				#endif
				dir.AddFile(tfinfo);
			}
		}

		return root;
	}

	private void Refresh(StatusData status)
	{
		Assert.IsNotNull(status);

		var stagedRoot   = BreakIntoTree(status.StagedFiles.Values,   StagedStatus.Staged);
		var unstagedRoot = BreakIntoTree(status.UnstagedFiles.Values, StagedStatus.Unstaged);
		lock(SyncRoot)
		{
			var stagedChanged   = Merge(_stagedLookup,   _stagedRoot,   status.StagedFiles,   stagedRoot);
			var unstagedChanged = Merge(_unstagedLookup, _unstagedRoot, status.UnstagedFiles, unstagedRoot);
			var countsChanged   = UpdateCounts(status);
			if(stagedChanged || unstagedChanged || countsChanged)
			{
				InvokeChanged();
			}
		}
	}

	/// <summary>Update status.</summary>
	public void Refresh()
	{
		StatusData status;
		var request = new QueryStatusRequest();
		using(Repository.Monitor?.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			status = Repository.Accessor.QueryStatus
				.Invoke(request);
		}
		Refresh(status);
	}

	/// <summary>Update status.</summary>
	public async Task RefreshAsync()
	{
		StatusData status;
		var request = new QueryStatusRequest();
		using(Repository.Monitor?.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			status = await Repository.Accessor.QueryStatus
				.InvokeAsync(request)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		Refresh(status);
	}

	private bool UpdateCounts(StatusData status)
	{
		Assert.IsNotNull(status);

		var countsChanged =
			_stagedAddedCount       != status.StagedAddedCount       ||
			_stagedModifiedCount    != status.StagedModifiedCount    ||
			_stagedRemovedCount     != status.StagedRemovedCount     ||
			_unmergedCount          != status.UnmergedCount          ||
			_unstagedUntrackedCount != status.UnstagedUntrackedCount ||
			_unstagedModifiedCount  != status.UnstagedModifiedCount  ||
			_unstagedRemovedCount   != status.UnstagedRemovedCount;

		if(!countsChanged) return false;

		_stagedAddedCount       = status.StagedAddedCount;
		_stagedModifiedCount    = status.StagedModifiedCount;
		_stagedRemovedCount     = status.StagedRemovedCount;
		_unmergedCount          = status.UnmergedCount;
		_unstagedUntrackedCount = status.UnstagedUntrackedCount;
		_unstagedModifiedCount  = status.UnstagedModifiedCount;
		_unstagedRemovedCount   = status.UnstagedRemovedCount;

		return true;
	}

	private void Update(TreeDirectoryData sourceDirectory, TreeDirectory targetDirectory,
		Dictionary<string, TreeFile> lookup)
	{
		Assert.IsNotNull(sourceDirectory);
		Assert.IsNotNull(targetDirectory);
		Assert.IsNotNull(lookup);

		int existsLength  = Math.Max(targetDirectory.Directories.Count, targetDirectory.Files.Count);
		int matchedLength = Math.Max(sourceDirectory.Directories.Count, sourceDirectory.Files.Count);

		var exists  = System.Buffers.ArrayPool<bool>.Shared.Rent(existsLength);
		var matched = System.Buffers.ArrayPool<bool>.Shared.Rent(matchedLength);
		try
		{
			#region update subdirectories

			Array.Clear(exists,  0, targetDirectory.Directories.Count);
			Array.Clear(matched, 0, sourceDirectory.Directories.Count);
			for(int i = 0; i < targetDirectory.Directories.Count; ++i)
			{
				var targetSubDirectory = targetDirectory.Directories[i];
				for(int j = 0; j < sourceDirectory.Directories.Count; ++j)
				{
					if(matched[j]) continue;

					var sourceSubDirectory = sourceDirectory.Directories[j];
					if(targetSubDirectory.Name == sourceSubDirectory.ShortName)
					{
						exists[i]  = true;
						matched[j] = true;
						Update(sourceSubDirectory, targetSubDirectory, lookup);
						break;
					}
				}
			}
			for(int i = targetDirectory.Directories.Count - 1; i >= 0; --i)
			{
				if(exists[i]) continue;

				var directory = targetDirectory.Directories[i];
				targetDirectory.RemoveDirectoryAt(i);
				InvokeRemovedDirectory(directory);
			}
			for(int i = 0; i < sourceDirectory.Directories.Count; ++i)
			{
				if(matched[i]) continue;

				var directory = ObjectFactories.CreateTreeDirectory(Repository, sourceDirectory.Directories[i]);
				targetDirectory.AddDirectory(directory);
				InvokeAddedDirectory(directory);
			}

			#endregion

			#region update files

			Array.Clear(exists,  0, targetDirectory.Files.Count);
			Array.Clear(matched, 0, sourceDirectory.Files.Count);
			for(int i = 0; i < targetDirectory.Files.Count; ++i)
			{
				var targetFile = targetDirectory.Files[i];
				for(int j = 0; j < sourceDirectory.Files.Count; ++j)
				{
					if(matched[j]) continue;

					var sourceFile = sourceDirectory.Files[j];
					if(targetFile.Name == sourceFile.ShortName)
					{
						exists[i]  = true;
						matched[j] = true;
						break;
					}
				}
			}
			for(int i = targetDirectory.Files.Count - 1; i >= 0; --i)
			{
				if(exists[i]) continue;

				var file = targetDirectory.Files[i];
				targetDirectory.RemoveFileAt(i);
				InvokeRemovedFile(file);
			}
			for(int i = 0; i < sourceDirectory.Files.Count; ++i)
			{
				if(matched[i]) continue;

				var treeFile = lookup[sourceDirectory.Files[i].Name];
				targetDirectory.AddFile(treeFile);
				InvokeNewFile(treeFile);
			}

			#endregion
		}
		finally
		{
			System.Buffers.ArrayPool<bool>.Shared.Return(exists);
			System.Buffers.ArrayPool<bool>.Shared.Return(matched);
		}
	}

	private bool Merge(
		Dictionary<string, TreeFile>     oldLookup, TreeDirectory     oldRoot,
		Dictionary<string, TreeFileData> newLookup, TreeDirectoryData newRoot)
	{
		bool changed = false;
		var toRemove = default(List<string>);

		foreach(var oldFileKvp in oldLookup)
		{
#if NETCOREAPP
			if(newLookup.Remove(oldFileKvp.Key, out var file))
#else
			if(newLookup.TryGetValue(oldFileKvp.Key, out var file))
#endif
			{
#if !NETCOREAPP
				newLookup.Remove(oldFileKvp.Key);
#endif
				ObjectFactories.UpdateTreeFile(oldFileKvp.Value, file);
			}
			else
			{
				(toRemove ??= []).Add(oldFileKvp.Key);
			}
		}

		if(toRemove is not null)
		{
			changed = true;
			foreach(var name in toRemove)
			{
				oldLookup.Remove(name);
			}
		}

		if(newLookup.Count != 0)
		{
			changed = true;
			foreach(var newFileKvp in newLookup)
			{
				oldLookup.Add(newFileKvp.Key, ObjectFactories.CreateTreeFile(Repository, newFileKvp.Value));
			}
		}

		if(changed)
		{
			Update(newRoot, oldRoot, oldLookup);
		}

		return changed;
	}

	public TreeFile? TryGetStaged(string path)
	{
		lock(SyncRoot)
		{
			if(_stagedLookup.TryGetValue(path, out var file))
			{
				return file;
			}
		}
		return null;
	}

	public TreeFile? TryGetUnstaged(string path)
	{
		lock(SyncRoot)
		{
			if(_unstagedLookup.TryGetValue(path, out var file))
			{
				return file;
			}
		}
		return null;
	}

	public void RevertPaths(IEnumerable<string>? paths)
	{
		if(paths is null) return;
		var pathsList = paths.ToList();
		if(pathsList.Count == 0) return;

		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.CheckoutFiles.Invoke(
					new CheckoutFilesRequest(pathsList)
					{
						Mode = CheckoutFileMode.IgnoreUnmergedEntries,
					});
			}
		}
		finally
		{
			Refresh();
		}
	}

	#region add

	internal void Stage(TreeItem item, AddFilesMode mode = AddFilesMode.All)
	{
		Verify.Argument.IsValidGitObject(item, Repository);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			Repository.Accessor.AddFiles.Invoke(
				new AddFilesRequest(mode, item.RelativePath));
		}
		Refresh();
	}

	internal async Task StageAsync(TreeItem item, AddFilesMode mode = AddFilesMode.All)
	{
		Verify.Argument.IsValidGitObject(item, Repository);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesRequest(mode, item.RelativePath))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Stage(string? pattern = null, bool includeUntracked = false, bool includeIgnored = false)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles.Invoke(
					new AddFilesRequest(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
					{
						Force = includeIgnored,
					});
			}
		}
		finally
		{
			Refresh();
		}
	}

	public async Task StageAsync(string? pattern = null, bool includeUntracked = false, bool includeIgnored = false)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				var request = new AddFilesRequest(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
				{
					Force = includeIgnored,
				};
				await Repository.Accessor.AddFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private string[]? GetPatterns(IReadOnlyList<TreeItem> items)
	{
		Verify.Argument.IsNotNull(items);
		if(items.Count == 0) return null;
		Verify.Argument.HasNoNullItems(items);
		var patterns = new string[items.Count];
		int id = 0;
		foreach(var item in items)
		{
			Verify.Argument.IsTrue(item.Repository == Repository, nameof(items),
				Resources.ExcAllObjectsMustBeHandledByThisRepository.UseAsFormat(nameof(items)));
			patterns[id++] = item.RelativePath;
		}
		return patterns;
	}

	public void Stage(IReadOnlyList<TreeItem> items)
	{
		if(GetPatterns(items) is { Length: > 0 } patterns)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles.Invoke(
					new AddFilesRequest(AddFilesMode.All, patterns));
			}
		}
		Refresh();
	}

	public async Task StageAsync(IReadOnlyList<TreeItem> items)
	{
		if(GetPatterns(items) is { Length: > 0 } patterns)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				var request = new AddFilesRequest(AddFilesMode.All, patterns);
				await Repository.Accessor.AddFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public void StageAll()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			Repository.Accessor.AddFiles.Invoke(
				new AddFilesRequest(AddFilesMode.All, "."));
		}
		Refresh();
	}

	public async Task StageAllAsync()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesRequest(AddFilesMode.All, "."))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public void StageUpdated()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			Repository.Accessor.AddFiles.Invoke(
				new AddFilesRequest(AddFilesMode.Update, "."));
		}
		Refresh();
	}

	public async Task StageUpdatedAsync()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesRequest(AddFilesMode.Update, "."))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	private static AddFilesRequest GetAddFilesParameters(string? pattern, bool includeUntracked, bool includeIgnored)
		=> new(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
		{
			Force = includeIgnored,
		};

	public async Task<IList<TreeFile>> GetFilesToAddAsync(string? pattern, bool includeUntracked, bool includeIgnored,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		progress?.Report(new(Resources.StrLookingForFiles.AddEllipsis()));
		IList<TreeFileData> files;
		var request = GetAddFilesParameters(pattern, includeUntracked, includeIgnored);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = await Repository.Accessor.QueryFilesToAdd
				.InvokeAsync(request, progress, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		var result = new List<TreeFile>(capacity: files.Count);
		foreach(var treeFileData in files)
		{
			result.Add(ObjectFactories.CreateTreeFile(Repository, treeFileData));
		}
		return result;
	}

	#endregion

	#region unstage

	public void UnstageAll()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			if(!Repository.IsEmpty)
			{
				Repository.Accessor.ResetFiles.Invoke(
					new ResetFilesRequest("."));
			}
			else
			{
				Repository.Accessor.RemoveFiles.Invoke(
					new RemoveFilesRequest(".")
					{
						Cached = true,
						Recursive = true,
					});
			}
		}
		Refresh();
	}

	public async Task UnstageAllAsync()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			if(!Repository.IsEmpty)
			{
				var request = new ResetFilesRequest(".");
				await Repository.Accessor.ResetFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				var request = new RemoveFilesRequest(".")
				{
					Cached    = true,
					Recursive = true,
				};
				await Repository.Accessor.RemoveFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	internal void Unstage(TreeItem item)
	{
		Verify.Argument.IsValidGitObject(item, Repository, nameof(item));

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			if(!Repository.IsEmpty)
			{
				Repository.Accessor.ResetFiles.Invoke(
					new ResetFilesRequest(item.RelativePath));
			}
			else
			{
				Repository.Accessor.RemoveFiles.Invoke(
					new RemoveFilesRequest(item.RelativePath)
					{
						Cached = true,
						Recursive = true,
					});
			}
		}
		Refresh();
	}

	internal async Task UnstageAsync(TreeItem item)
	{
		Verify.Argument.IsValidGitObject(item, Repository, nameof(item));

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			if(!Repository.IsEmpty)
			{
				var request = new ResetFilesRequest(item.RelativePath);
				await Repository.Accessor.ResetFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				var request = new RemoveFilesRequest(item.RelativePath)
				{
					Cached    = true,
					Recursive = true,
				};
				await Repository.Accessor.RemoveFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Unstage(IReadOnlyList<TreeItem> items)
	{
		if(GetPatterns(items) is { Length: > 0 } patterns)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				if(!Repository.Head.IsEmpty)
				{
					Repository.Accessor.ResetFiles.Invoke(
						new ResetFilesRequest(patterns));
				}
				else
				{
					Repository.Accessor.RemoveFiles.Invoke(
						new RemoveFilesRequest(patterns)
						{
							Cached = true,
							Recursive = true,
						});
				}
			}
			Refresh();
		}
	}

	public async Task UnstageAsync(IReadOnlyList<TreeItem> items)
	{
		if(GetPatterns(items) is { Length: > 0 } patterns)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				if(!Repository.Head.IsEmpty)
				{
					var request = new ResetFilesRequest(patterns);
					await Repository.Accessor.ResetFiles
						.InvokeAsync(request)
						.ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					var request = new RemoveFilesRequest(patterns)
					{
						Cached    = true,
						Recursive = true,
					};
					await Repository.Accessor.RemoveFiles
						.InvokeAsync(request)
						.ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	#endregion

	#region reset

	/// <summary>Reset changes in working tree.</summary>
	/// <param name="mode">Reset mode</param>
	/// <exception cref="T:gitter.Git.GitException">git reset failed.</exception>
	public void Reset(ResetMode mode)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.Reset.Invoke(
					new ResetRequest(mode));
			}
		}
		finally
		{
			Refresh();
		}
	}

	/// <summary>Reset changes in working tree.</summary>
	/// <param name="mode">Reset mode</param>
	/// <exception cref="T:gitter.Git.GitException">git reset failed.</exception>
	public async Task ResetAsync(ResetMode mode)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				await Repository.Accessor.Reset
					.InvokeAsync(new ResetRequest(mode))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	#endregion

	#region clean

	private static CleanFilesRequest GetCleanFilesRequest(string? includePattern, string? excludePattern, CleanFilesMode mode, bool removeDirectories, bool force = false)
		=> new(Many.OneOrNone(includePattern))
		{
			ExcludePatterns   = Many.OneOrNone(excludePattern),
			Mode              = mode,
			RemoveDirectories = removeDirectories,
			Force             = force,
		};

	private TreeItem[] RestoreFilesToCleanList(IList<string> files)
	{
		Assert.IsNotNull(files);

		if(files.Count == 0) return Preallocated<TreeItem>.EmptyArray;

		var result = new TreeItem[files.Count];
		for(int i = 0; i < result.Length; ++i)
		{
			var item = files[i];
			if(item.EndsWith('/'))
			{
				var name = item;
				if(name.Length != 0)
				{
					name = name.Substring(0, name.Length - 1);
				}
				result[i] = new TreeDirectory(Repository,
					item, null, FileStatus.Cached, name);
			}
			else
			{
				result[i] = new TreeFile(Repository,
					item, null, FileStatus.Cached, item);
			}
		}
		return result;
	}

	/// <summary>Returns a list of files which will be removed by a clean function.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	/// <returns>Files that will be removed by a Clean() call.</returns>
	public IReadOnlyList<TreeItem> GetFilesToClean(string? includePattern, string? excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		IList<string> files;
		var request = GetCleanFilesRequest(includePattern, excludePattern, mode, removeDirectories);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = Repository.Accessor.QueryFilesToClean.Invoke(request);
		}
		return RestoreFilesToCleanList(files);
	}

	/// <summary>Returns a list of files which will be removed by a clean function.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	/// <param name="progress">Progress tracker.</param>
	/// <param name="cancellationToken">Operation cancellation token.</param>
	/// <returns>Files that will be removed by a Clean() call.</returns>
	public async Task<IReadOnlyList<TreeItem>> GetFilesToCleanAsync(string? includePattern, string? excludePattern, CleanFilesMode mode, bool removeDirectories,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		IList<string> files;
		progress?.Report(new OperationProgress(Resources.StrsLookingForFiles.AddEllipsis()));
		var request = GetCleanFilesRequest(includePattern, excludePattern, mode, removeDirectories);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = await Repository
				.Accessor
				.QueryFilesToClean
				.InvokeAsync(request, progress, cancellationToken);
		}
		return RestoreFilesToCleanList(files);
	}

	/// <summary>Remove untracked and/or ignored files and (optionally) directories.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	public void Clean(string? includePattern, string? excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		var request = GetCleanFilesRequest(includePattern, excludePattern, mode, removeDirectories, force: true);
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.CleanFiles.Invoke(request);
			}
		}
		finally
		{
			Refresh();
		}
	}

	/// <summary>Remove untracked and/or ignored files and (optionally) directories.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	public async Task CleanAsync(string? includePattern, string? excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		var request = GetCleanFilesRequest(includePattern, excludePattern, mode, removeDirectories, force: true);
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				await Repository.Accessor.CleanFiles
					.InvokeAsync(request)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	#endregion

	#region apply

	/// <summary>Apply patch.</summary>
	/// <param name="patchSource">Patch source.</param>
	/// <param name="applyTo">Patch target.</param>
	/// <param name="reverse">Reverse patches.</param>
	public void ApplyPatch(IPatchSource patchSource, ApplyPatchTo applyTo, bool reverse)
	{
		Verify.Argument.IsNotNull(patchSource);

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			using var patch = patchSource.PreparePatchFile();
			Repository.Accessor.ApplyPatch.Invoke(
				new ApplyPatchRequest
				{
					Patches = patch.FileName,
					ApplyTo = applyTo,
					Reverse = reverse,
				});
		}
		Refresh();
	}

	/// <summary>Apply multiple patches.</summary>
	/// <param name="patchSources">Patch sources.</param>
	/// <param name="applyTo">Patch target.</param>
	/// <param name="reverse">Reverse patches.</param>
	public void ApplyPatches(IEnumerable<IPatchSource> patchSources, ApplyPatchTo applyTo, bool reverse)
	{
		Verify.Argument.IsNotNull(patchSources);

		var files = new List<IPatchFile>();
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			try
			{
				foreach(var patchSource in patchSources)
				{
					files.Add(patchSource.PreparePatchFile());
				}
				if(files.Count != 0)
				{
					var fileNames = new string[files.Count];
					for(int i = 0; i < fileNames.Length; ++i)
					{
						fileNames[i] = files[i].FileName;
					}
					Repository.Accessor.ApplyPatch.Invoke(
						new ApplyPatchRequest
						{
							Patches = fileNames,
							ApplyTo = applyTo,
							Reverse = reverse,
						});
				}
			}
			finally
			{
				foreach(var patchFile in files)
				{
					patchFile.Dispose();
				}
			}
		}
		Refresh();
	}

	#endregion

	#region commit

	private string SaveMessageForCommit(string message)
	{
		var fileName = Path.Combine(
			Repository.GitDirectory,
			GitConstants.CommitMessageFileName);
		File.WriteAllText(fileName, message);
		return fileName;
	}

	private static void DeleteMessageAfterCommit(string fileName)
	{
		try
		{
			File.Delete(fileName);
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}

	/*
	 * git commit [-a | --interactive] [-s] [-v] [-u<mode>] [--amend] [--dry-run] [(-c | -C) <commit>]
	 *			  [-F <file> | -m <msg>] [--reset-author] [--allow-empty] [--no-verify] [-e] [--author=<author>]
	 *			  [--date=<date>] [--cleanup=<mode>] [--status | --no-status] [--] [[-i | -o ]<file>…]  */

	public CommitResult Commit(string message, bool amend = false)
	{
		Verify.Argument.IsNotNull(message);

		var currentBranch = Repository.Head.Pointer as Branch;
		var output        = default(string);
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.Checkout))
		{
			var fileName = SaveMessageForCommit(message);
			var request = new CommitRequest
			{
				Message = MessageSpecification.FromFile(fileName),
				Amend   = amend,
			};
			output = Repository.Accessor.Commit.Invoke(request);
			DeleteMessageAfterCommit(fileName);
		}
		return AfterCommit(currentBranch, output);
	}

	private CommitResult AfterCommit(Branch? currentBranch, string output)
	{
		Revision commit;
		if(currentBranch is not null)
		{
			var oldHeadRev = currentBranch.Revision;
			currentBranch.Refresh();
			commit = currentBranch.Revision!;
			if(commit != oldHeadRev)
			{
				Repository.OnCommitCreated(commit);
			}
		}
		else
		{
			var oldHeadRev = Repository.Head.Revision;
			Repository.Head.Refresh();
			commit = Repository.Head.Revision!;
			if(commit != oldHeadRev)
			{
				Repository.OnCommitCreated(commit);
			}
		}
		Repository.Head.NotifyReflogRecordAdded();
		Repository.OnStateChanged();
		Refresh();
		var result = new CommitResult(commit, output);
		OnCommitted(result);
		return result;
	}

	public async Task<CommitResult> CommitAsync(string message, bool amend = false,
		IProgress<OperationProgress>? progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(message);

		var currentBranch = Repository.Head.Pointer as Branch;
		var output        = default(string);
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.Checkout))
		{
			var fileName = SaveMessageForCommit(message);
			var request = new CommitRequest
			{
				Message = MessageSpecification.FromFile(fileName),
				Amend   = amend,
			};
			output = await Repository.Accessor.Commit
				.InvokeAsync(request, progress, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			DeleteMessageAfterCommit(fileName);
		}
		return await AfterCommitAsync(currentBranch, output)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<CommitResult> AfterCommitAsync(Branch? currentBranch, string output)
	{
		Revision commit;
		if(currentBranch is not null)
		{
			var oldHeadRev = currentBranch.Revision;
			await currentBranch
				.RefreshAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			commit = currentBranch.Revision!;
			if(commit != oldHeadRev)
			{
				Repository.OnCommitCreated(commit);
			}
		}
		else
		{
			var oldHeadRev = Repository.Head.Revision;
			await Repository.Head
				.RefreshAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			commit = Repository.Head.Revision!;
			if(commit != oldHeadRev)
			{
				Repository.OnCommitCreated(commit);
			}
		}
		Repository.Head.NotifyReflogRecordAdded();
		Repository.OnStateChanged();
		await RefreshAsync()
			.ConfigureAwait(continueOnCapturedContext: false);
		var result = new CommitResult(commit, output);
		OnCommitted(result);
		return result;
	}

	public string LoadCommitMessage()
	{
		var fileName = Path.Combine(
			Repository.GitDirectory,
			GitConstants.CommitMessageFileName);
		try
		{
			return File.Exists(fileName)
				? File.ReadAllText(fileName)
				: string.Empty;
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
			return string.Empty;
		}
	}

	public void SaveCommitMessage(string? message)
	{
		var fileName = Path.Combine(
			Repository.GitDirectory,
			GitConstants.CommitMessageFileName);
		try
		{
			if(string.IsNullOrWhiteSpace(message))
			{
				if(File.Exists(fileName)) File.Delete(fileName);
			}
			else
			{
				File.WriteAllText(fileName, message);
			}
		}
		catch(Exception exc) when(!exc.IsCritical)
		{
		}
	}

	#endregion

	#region diff

	public IIndexDiffSource GetDiffSource(bool cached, Many<string> paths = default)
		=> paths.IsEmpty
			? new IndexChangesDiffSource(Repository, cached)
			: new IndexChangesDiffSource(Repository, cached, paths);

	#endregion
}
