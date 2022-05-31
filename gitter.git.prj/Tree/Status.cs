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
using System.Text;
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

	public event EventHandler Changed;

	public event EventHandler<TreeFileEventArgs> NewStagedFile;
	public event EventHandler<TreeDirectoryEventArgs> NewStagedFolder;

	public event EventHandler<TreeFileEventArgs> RemovedStagedFile;
	public event EventHandler<TreeDirectoryEventArgs> RemovedStagedFolder;

	public event EventHandler<TreeFileEventArgs> NewUnstagedFile;
	public event EventHandler<TreeDirectoryEventArgs> NewUnstagedFolder;

	public event EventHandler<TreeFileEventArgs> RemovedUnstagedFile;
	public event EventHandler<TreeDirectoryEventArgs> RemovedUnstagedFolder;

	public event EventHandler<CommitResultEventArgs> Committed;

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
		=> Committed?.Invoke(this, new CommitResultEventArgs(commitResult));

	#endregion

	#region Data

	private readonly Dictionary<string, TreeFile> _unstagedPlainList;
	private readonly Dictionary<string, TreeFile> _stagedPlainList;

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
		_unstagedPlainList = new Dictionary<string, TreeFile>();
		_stagedPlainList = new Dictionary<string, TreeFile>();
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

	public ICollection<TreeFile> StagedFiles => _stagedPlainList.Values;

	public TreeDirectory StagedRoot => _stagedRoot;

	public ICollection<TreeFile> UnstagedFiles => _unstagedPlainList.Values;

	public TreeDirectory UnstagedRoot => _unstagedRoot;

	public int UnstagedUntrackedCount => _unstagedUntrackedCount;

	public int UnstagedModifiedCount => _unstagedModifiedCount;

	public int UnstagedRemovedCount => _unstagedRemovedCount;

	public int UnmergedCount => _unmergedCount;

	public int StagedAddedCount => _stagedAddedCount;

	public int StagedModifiedCount => _stagedModifiedCount;

	public int StagedRemovedCount => _stagedRemovedCount;

	/// <summary>Object used for cross-thread synchronization.</summary>
	public object SyncRoot { get; } = new();

	#endregion

	private static TreeDirectoryData BreakIntoTree(IReadOnlyDictionary<string, TreeFileData> files, StagedStatus stagedStatus)
	{
		var root = new TreeDirectoryData("", "", null, FileStatus.Cached, stagedStatus);
		var dirs = new Dictionary<string, TreeDirectoryData>();
		var path = new StringBuilder();

		foreach(var tfinfo in files.Values)
		{
			var parent = root;
			var name = tfinfo.Name;
			var nameLength = name.Length;
			//if(name.EndsWith("/"))
			//{
			//    --nameLength;
			//    name = name.Substring(0, nameLength);
			//}
			int slashPos = 0;
			path.Clear();
			while(slashPos != -1 && slashPos < nameLength)
			{
				bool isFile;
				int endOfPathName = name.IndexOf('/', slashPos);
				if(endOfPathName == -1)
				{
					endOfPathName = nameLength;
					isFile = true;
				}
				else
				{
					isFile = false;
				}
				string partName;
				if(isFile)
				{
					partName = slashPos == 0 ?
						name :
						name.Substring(slashPos, endOfPathName - slashPos);
					tfinfo.ShortName = partName;
					parent.AddFile(tfinfo);
					break;
				}
				else
				{
					partName = name.Substring(slashPos, endOfPathName - slashPos);
					path.Append(partName);
					path.Append('/');
					string currentPath = path.ToString();
					if(!dirs.TryGetValue(currentPath, out var wtDirectory))
					{
						wtDirectory = new TreeDirectoryData(currentPath, partName, parent, FileStatus.Cached, stagedStatus);
						dirs.Add(currentPath, wtDirectory);
						parent.AddDirectory(wtDirectory);
					}
					parent = wtDirectory;
				}
				slashPos = endOfPathName + 1;
			}
		}

		return root;
	}

	private void Refresh(StatusData status)
	{
		var stagedRoot   = BreakIntoTree(status.StagedFiles,   StagedStatus.Staged);
		var unstagedRoot = BreakIntoTree(status.UnstagedFiles, StagedStatus.Unstaged);

		bool m1, m2, m3;
		lock(SyncRoot)
		{
			m1 = Merge(_stagedPlainList,   _stagedRoot,   status.StagedFiles,   stagedRoot);
			m2 = Merge(_unstagedPlainList, _unstagedRoot, status.UnstagedFiles, unstagedRoot);
			m3 = _stagedAddedCount			!= status.StagedAddedCount ||
					_stagedModifiedCount		!= status.StagedModifiedCount ||
					_stagedRemovedCount		!= status.StagedRemovedCount ||
					_unmergedCount				!= status.UnmergedCount ||
					_unstagedUntrackedCount	!= status.UnstagedUntrackedCount ||
					_unstagedModifiedCount		!= status.UnstagedModifiedCount ||
					_unstagedRemovedCount		!= status.UnstagedRemovedCount;
			if(m3)
			{
				_stagedAddedCount		= status.StagedAddedCount;
				_stagedModifiedCount	= status.StagedModifiedCount;
				_stagedRemovedCount		= status.StagedRemovedCount;
				_unmergedCount			= status.UnmergedCount;
				_unstagedUntrackedCount	= status.UnstagedUntrackedCount;
				_unstagedModifiedCount	= status.UnstagedModifiedCount;
				_unstagedRemovedCount	= status.UnstagedRemovedCount;
			}

			if(m1 || m2 || m3)
			{
				InvokeChanged();
			}
		}
	}

	/// <summary>Update status.</summary>
	public void Refresh()
	{
		StatusData status;
		var parameters = new QueryStatusParameters();
		using(Repository.Monitor?.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			status = Repository.Accessor.QueryStatus
				.Invoke(parameters);
		}
		Refresh(status);
	}

	/// <summary>Update status.</summary>
	public async Task RefreshAsync()
	{
		StatusData status;
		var parameters = new QueryStatusParameters();
		using(Repository.Monitor?.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.WorktreeUpdated))
		{
			status = await Repository.Accessor.QueryStatus
				.InvokeAsync(parameters)
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		Refresh(status);
	}

	private static void Remove(TreeFile file)
	{
		var from = file.Parent;
		from.Files.Remove(file);
		if(from.Files.Count == 0 && from.Directories.Count == 0 && from.Parent != null)
		{
			Remove(from);
		}
	}

	private static void Remove(TreeDirectory folder)
	{
		var from = folder.Parent;
		from.Directories.Remove(folder);
		if(from.Files.Count == 0 && from.Directories.Count == 0 && from.Parent != null)
		{
			Remove(from);
		}
	}

	private void Update(TreeDirectoryData sourceDirectory, TreeDirectory targetDirectory)
	{
		bool[] existance;
		bool[] matched;

		#region update subdirectories

		existance	= new bool[targetDirectory.Directories.Count];
		matched		= new bool[sourceDirectory.Directories.Count];
		for(int i = 0; i < targetDirectory.Directories.Count; ++i)
		{
			var targetSubDirectory = targetDirectory.Directories[i];
			for(int j = 0; j < sourceDirectory.Directories.Count; ++j)
			{
				if(!matched[j])
				{
					var sourceSubDirectory = sourceDirectory.Directories[j];
					if(targetSubDirectory.Name == sourceSubDirectory.ShortName)
					{
						existance[i]	= true;
						matched[j]		= true;
						Update(sourceSubDirectory, targetSubDirectory);
						break;
					}
				}
			}
		}
		for(int i = targetDirectory.Directories.Count - 1; i >= 0; --i)
		{
			if(!existance[i])
			{
				var directory = targetDirectory.Directories[i];
				targetDirectory.RemoveDirectoryAt(i);
				InvokeRemovedDirectory(directory);
			}
		}
		for(int i = 0; i < sourceDirectory.Directories.Count; ++i)
		{
			if(!matched[i])
			{
				var directory = ObjectFactories.CreateTreeDirectory(Repository, sourceDirectory.Directories[i]);
				targetDirectory.AddDirectory(directory);
				InvokeAddedDirectory(directory);
			}
		}

		#endregion

		#region update files

		existance	= new bool[targetDirectory.Files.Count];
		matched		= new bool[sourceDirectory.Files.Count];
		for(int i = 0; i < targetDirectory.Files.Count; ++i)
		{
			var targetFile = targetDirectory.Files[i];
			for(int j = 0; j < sourceDirectory.Files.Count; ++j)
			{
				if(!matched[j])
				{
					var sourceFile = sourceDirectory.Files[j];
					if(targetFile.Name == sourceFile.ShortName)
					{
						existance[i]	= true;
						matched[j]		= true;
						targetFile.Status = sourceFile.FileStatus;
						break;
					}
				}
			}
		}
		for(int i = targetDirectory.Files.Count - 1; i >= 0; --i)
		{
			if(!existance[i])
			{
				var file = targetDirectory.Files[i];
				targetDirectory.RemoveFileAt(i);
				InvokeRemovedFile(file);
			}
		}
		for(int i = 0; i < sourceDirectory.Files.Count; ++i)
		{
			if(!matched[i])
			{
				var treeFile = ObjectFactories.CreateTreeFile(Repository, sourceDirectory.Files[i]);
				targetDirectory.AddFile(treeFile);
				InvokeNewFile(treeFile);
			}
		}

		#endregion
	}

	private bool Merge(
		Dictionary<string, TreeFile>	  oldPlainList, TreeDirectory	  oldRoot,
		Dictionary<string, TreeFileData> newPlainList,	TreeDirectoryData newRoot)
	{
		bool res = false;
		var removeList = new List<string>();

		foreach(var oldFileKvp in oldPlainList)
		{
			if(newPlainList.TryGetValue(oldFileKvp.Key, out var file))
			{
				newPlainList.Remove(oldFileKvp.Key);
				ObjectFactories.UpdateTreeFile(oldFileKvp.Value, file);
			}
			else
			{
				removeList.Add(oldFileKvp.Key);
				res = true;
			}
		}

		foreach(var s in removeList)
		{
			oldPlainList.Remove(s);
		}

		if(newPlainList.Count != 0)
		{
			res = true;
			foreach(var newFileKvp in newPlainList)
			{
				oldPlainList.Add(newFileKvp.Key, ObjectFactories.CreateTreeFile(Repository, newFileKvp.Value));
			}
		}

		Update(newRoot, oldRoot);
		return res;
	}

	public TreeFile TryGetStaged(string path)
	{
		lock(SyncRoot)
		{
			if(_stagedPlainList.TryGetValue(path, out var file))
			{
				return file;
			}
		}
		return null;
	}

	public TreeFile TryGetUnstaged(string path)
	{
		lock(SyncRoot)
		{
			if(_unstagedPlainList.TryGetValue(path, out var file))
			{
				return file;
			}
		}
		return null;
	}

	public void RevertPaths(IEnumerable<string> paths)
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
					new CheckoutFilesParameters(pathsList)
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
		Verify.Argument.IsValidGitObject(item, Repository, nameof(item));

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			Repository.Accessor.AddFiles.Invoke(
				new AddFilesParameters(mode, item.RelativePath));
		}
		Refresh();
	}

	internal async Task StageAsync(TreeItem item, AddFilesMode mode = AddFilesMode.All)
	{
		Verify.Argument.IsValidGitObject(item, Repository, nameof(item));

		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesParameters(mode, item.RelativePath))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Stage(string pattern = null, bool includeUntracked = false, bool includeIgnored = false)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles.Invoke(
					new AddFilesParameters(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
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

	public async Task StageAsync(string pattern = null, bool includeUntracked = false, bool includeIgnored = false)
	{
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				var parameters = new AddFilesParameters(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
				{
					Force = includeIgnored,
				};
				await Repository.Accessor.AddFiles
					.InvokeAsync(parameters)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private string[] GetPatterns(IReadOnlyList<TreeItem> items)
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
					new AddFilesParameters(AddFilesMode.All, patterns));
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
				var parameters = new AddFilesParameters(AddFilesMode.All, patterns);
				await Repository.Accessor.AddFiles
					.InvokeAsync(parameters)
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
				new AddFilesParameters(AddFilesMode.All, "."));
		}
		Refresh();
	}

	public async Task StageAllAsync()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesParameters(AddFilesMode.All, "."))
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
				new AddFilesParameters(AddFilesMode.Update, "."));
		}
		Refresh();
	}

	public async Task StageUpdatedAsync()
	{
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated))
		{
			await Repository.Accessor.AddFiles
				.InvokeAsync(new AddFilesParameters(AddFilesMode.Update, "."))
				.ConfigureAwait(continueOnCapturedContext: false);
		}
		await RefreshAsync().ConfigureAwait(continueOnCapturedContext: false);
	}

	private static AddFilesParameters GetAddFilesParameters(string pattern, bool includeUntracked, bool includeIgnored)
		=> new(includeUntracked ? AddFilesMode.All : AddFilesMode.Update, pattern)
		{
			Force = includeIgnored,
		};

	public async Task<IList<TreeFile>> GetFilesToAddAsync(string pattern, bool includeUntracked, bool includeIgnored,
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		progress?.Report(new OperationProgress(Resources.StrLookingForFiles.AddEllipsis()));
		IList<TreeFileData> files;
		var parameters = GetAddFilesParameters(pattern, includeUntracked, includeIgnored);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = await Repository.Accessor.QueryFilesToAdd
				.InvokeAsync(parameters, progress, cancellationToken)
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
					new ResetFilesParameters("."));
			}
			else
			{
				Repository.Accessor.RemoveFiles.Invoke(
					new RemoveFilesParameters(".")
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
				var parameters = new ResetFilesParameters(".");
				await Repository.Accessor.ResetFiles
					.InvokeAsync(parameters)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				var parameters = new RemoveFilesParameters(".")
				{
					Cached    = true,
					Recursive = true,
				};
				await Repository.Accessor.RemoveFiles
					.InvokeAsync(parameters)
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
					new ResetFilesParameters(item.RelativePath));
			}
			else
			{
				Repository.Accessor.RemoveFiles.Invoke(
					new RemoveFilesParameters(item.RelativePath)
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
				var parameters = new ResetFilesParameters(item.RelativePath);
				await Repository.Accessor.ResetFiles
					.InvokeAsync(parameters)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				var parameters = new RemoveFilesParameters(item.RelativePath)
				{
					Cached    = true,
					Recursive = true,
				};
				await Repository.Accessor.RemoveFiles
					.InvokeAsync(parameters)
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
						new ResetFilesParameters(patterns));
				}
				else
				{
					Repository.Accessor.RemoveFiles.Invoke(
						new RemoveFilesParameters(patterns)
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
					var parameters = new ResetFilesParameters(patterns);
					await Repository.Accessor.ResetFiles
						.InvokeAsync(parameters)
						.ConfigureAwait(continueOnCapturedContext: false);
				}
				else
				{
					var parameters = new RemoveFilesParameters(patterns)
					{
						Cached    = true,
						Recursive = true,
					};
					await Repository.Accessor.RemoveFiles
						.InvokeAsync(parameters)
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
					new ResetParameters(mode));
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
					.InvokeAsync(new ResetParameters(mode))
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

	private static CleanFilesParameters GetCleanFilesParameters(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories, bool force = false)
		=> new CleanFilesParameters(includePattern)
		{
			ExcludePatterns   = new[] { excludePattern },
			Mode              = mode,
			RemoveDirectories = removeDirectories,
			Force             = force,
		};

	private IReadOnlyList<TreeItem> RestoreFilesToCleanList(IList<string> files)
	{
		Assert.IsNotNull(files);

		var result = new TreeItem[files.Count];
		for(int i = 0; i < result.Length; ++i)
		{
			var item = files[i];
			if(item.EndsWith("/"))
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
	public IReadOnlyList<TreeItem> GetFilesToClean(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		IList<string> files;
		var parameters = GetCleanFilesParameters(includePattern, excludePattern, mode, removeDirectories);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = Repository.Accessor.QueryFilesToClean.Invoke(parameters);
		}
		return RestoreFilesToCleanList(files);
	}

	/// <summary>Returns a list of files which will be removed by a clean function.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	/// <returns>Files that will be removed by a Clean() call.</returns>
	public async Task<IReadOnlyList<TreeItem>> GetFilesToCleanAsync(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories,
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		IList<string> files;
		progress?.Report(new OperationProgress(Resources.StrsLookingForFiles.AddEllipsis()));
		var parameters = GetCleanFilesParameters(includePattern, excludePattern, mode, removeDirectories);
		using(Repository.Monitor.BlockNotifications(RepositoryNotifications.IndexUpdated))
		{
			files = await Repository
				.Accessor
				.QueryFilesToClean
				.InvokeAsync(parameters, progress, cancellationToken);
		}
		return RestoreFilesToCleanList(files);
	}

	/// <summary>Remove untracked and/or ignored files and (optionally) directories.</summary>
	/// <param name="includePattern">Files to clean.</param>
	/// <param name="excludePattern">Files to save.</param>
	/// <param name="mode">Clean mode.</param>
	/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
	public void Clean(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		var parameters = GetCleanFilesParameters(includePattern, excludePattern, mode, removeDirectories, force: true);
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.CleanFiles.Invoke(parameters);
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
	public async Task CleanAsync(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories)
	{
		var parameters = GetCleanFilesParameters(includePattern, excludePattern, mode, removeDirectories, force: true);
		try
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				await Repository.Accessor.CleanFiles
					.InvokeAsync(parameters)
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
			using(var patch = patchSource.PreparePatchFile())
			{
				Repository.Accessor.ApplyPatch.Invoke(
					new ApplyPatchParameters
					{
						Patches = new[] { patch.FileName },
						ApplyTo = applyTo,
						Reverse = reverse,
					});
			}
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
						new ApplyPatchParameters
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
			var fileName = Path.Combine(
				Repository.GitDirectory,
				GitConstants.CommitMessageFileName);
			File.WriteAllText(fileName, message);
			output = Repository.Accessor.Commit.Invoke(
				new CommitParameters
				{
					MessageFileName = Path.Combine(
						Repository.GitDirectory, GitConstants.CommitMessageFileName),
					Amend = amend,
				});
			try
			{
				File.Delete(fileName);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}
		return AfterCommit(currentBranch, output);
	}

	private CommitResult AfterCommit(Branch currentBranch, string output)
	{
		Revision commit;
		if(currentBranch is not null)
		{
			var oldHeadRev = currentBranch.Revision;
			currentBranch.Refresh();
			commit = currentBranch.Revision;
			if(commit != oldHeadRev)
			{
				Repository.OnCommitCreated(commit);
			}
		}
		else
		{
			var oldHeadRev = Repository.Head.Revision;
			Repository.Head.Refresh();
			commit = Repository.Head.Revision;
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
		IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
	{
		Verify.Argument.IsNotNull(message);

		var currentBranch = Repository.Head.Pointer as Branch;
		var output        = default(string);
		using(Repository.Monitor.BlockNotifications(
			RepositoryNotifications.IndexUpdated,
			RepositoryNotifications.BranchChanged,
			RepositoryNotifications.Checkout))
		{
			var fileName = Path.Combine(
				Repository.GitDirectory,
				GitConstants.CommitMessageFileName);
			File.WriteAllText(fileName, message);
			var parameters = new CommitParameters
			{
				MessageFileName = Path.Combine(Repository.GitDirectory, GitConstants.CommitMessageFileName),
				Amend           = amend,
			};
			output = await Repository.Accessor.Commit
				.InvokeAsync(parameters, progress, cancellationToken)
				.ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				File.Delete(fileName);
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
		}
		return await AfterCommitAsync(currentBranch, output)
			.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<CommitResult> AfterCommitAsync(Branch currentBranch, string output)
	{
		Revision commit;
		if(currentBranch is not null)
		{
			var oldHeadRev = currentBranch.Revision;
			await currentBranch
				.RefreshAsync()
				.ConfigureAwait(continueOnCapturedContext: false);
			commit = currentBranch.Revision;
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
			commit = Repository.Head.Revision;
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
		catch(Exception exc) when(!exc.IsCritical())
		{
			return string.Empty;
		}
	}

	public void SaveCommitMessage(string message)
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
		catch(Exception exc) when(!exc.IsCritical())
		{
		}
	}

	#endregion

	#region diff

	public IIndexDiffSource GetDiffSource(bool cached, IEnumerable<string> paths = null)
		=> paths == null
			? new IndexChangesDiffSource(Repository, cached)
			: new IndexChangesDiffSource(Repository, cached, paths.ToList());

	#endregion
}
