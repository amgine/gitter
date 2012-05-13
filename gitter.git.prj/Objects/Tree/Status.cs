namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Text;
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

		private void InvokeChanged()
		{
			var handler = Changed;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		private void InvokeNewFile(TreeFile file)
		{
			var handler = file.StagedStatus == StagedStatus.Staged ? NewStagedFile : NewUnstagedFile;
			if(handler != null) handler(this, new TreeFileEventArgs(file));
		}

		private void InvokeAddedDirectory(TreeDirectory folder)
		{
			var handler = folder.StagedStatus == StagedStatus.Staged ? NewStagedFolder : NewUnstagedFolder;
			if(handler != null) handler(this, new TreeDirectoryEventArgs(folder));
		}

		private void InvokeRemovedFile(TreeFile file)
		{
			file.MarkAsDeleted();
			var handler = file.StagedStatus == StagedStatus.Staged ? RemovedStagedFile : RemovedUnstagedFile;
			if(handler != null) handler(this, new TreeFileEventArgs(file));
		}

		private void InvokeRemovedDirectory(TreeDirectory folder)
		{
			foreach(var subFolder in folder.Directories)
			{
				InvokeRemovedDirectory(subFolder);
			}
			folder.MarkAsDeleted();
			var handler = folder.StagedStatus == StagedStatus.Staged ? RemovedStagedFolder : RemovedUnstagedFolder;
			if(handler != null) handler(this, new TreeDirectoryEventArgs(folder));
			foreach(var file in folder.Files)
			{
				InvokeRemovedFile(file);
			}
		}

		#endregion

		#region Data

		private readonly object _syncRoot = new object();
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

		public ICollection<TreeFile> StagedFiles
		{
			get { return _stagedPlainList.Values; }
		}

		public TreeDirectory StagedRoot
		{
			get { return _stagedRoot; }
		}

		public ICollection<TreeFile> UnstagedFiles
		{
			get { return _unstagedPlainList.Values; }
		}

		public TreeDirectory UnstagedRoot
		{
			get { return _unstagedRoot; }
		}

		public int UnstagedUntrackedCount
		{
			get { return _unstagedUntrackedCount; }
		}

		public int UnstagedModifiedCount
		{
			get { return _unstagedModifiedCount; }
		}

		public int UnstagedRemovedCount
		{
			get { return _unstagedRemovedCount; }
		}

		public int UnmergedCount
		{
			get { return _unmergedCount; }
		}

		public int StagedAddedCount
		{
			get { return _stagedAddedCount; }
		}

		public int StagedModifiedCount
		{
			get { return _stagedModifiedCount; }
		}

		public int StagedRemovedCount
		{
			get { return _stagedRemovedCount; }
		}

		/// <summary>Object used for cross-thread synchronization.</summary>
		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		#endregion

		private static TreeDirectoryData BreakIntoTree(IDictionary<string, TreeFileData> files, StagedStatus stagedStatus)
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
						TreeDirectoryData wtDirectory;
						if(!dirs.TryGetValue(currentPath, out wtDirectory))
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

		/// <summary>Update status.</summary>
		public void Refresh()
		{
			StatusData status;
			var parameters = new QueryStatusParameters();
			if(Repository.Monitor != null)
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated))
				{
					status = Repository.Accessor.QueryStatus(parameters);
				}
			}
			else
			{
				status = Repository.Accessor.QueryStatus(parameters);
			}

			var stagedRoot		= BreakIntoTree(status.StagedFiles,		StagedStatus.Staged);
			var unstagedRoot	= BreakIntoTree(status.UnstagedFiles,	StagedStatus.Unstaged);

			bool m1, m2, m3;
			lock(SyncRoot)
			{
				m1 = Merge(_stagedPlainList,	_stagedRoot,	status.StagedFiles,		stagedRoot);
				m2 = Merge(_unstagedPlainList,	_unstagedRoot,	status.UnstagedFiles,	unstagedRoot);
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
			IDictionary<string, TreeFile>	  oldPlainList, TreeDirectory	  oldRoot,
			IDictionary<string, TreeFileData> newPlainList,	TreeDirectoryData newRoot)
		{
			bool res = false;
			var removeList = new List<string>();

			foreach(var oldFileKvp in oldPlainList)
			{
				TreeFileData file;
				if(newPlainList.TryGetValue(oldFileKvp.Key, out file))
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
			TreeFile file;
			lock(SyncRoot)
			{
				if(_stagedPlainList.TryGetValue(path, out file))
				{
					return file;
				}
			}
			return null;
		}

		public TreeFile TryGetUnstaged(string path)
		{
			TreeFile file;
			lock(SyncRoot)
			{
				if(_unstagedPlainList.TryGetValue(path, out file))
				{
					return file;
				}
			}
			return null;
		}

		#region add

		internal void Stage(TreeItem item)
		{
			Stage(item, AddFilesMode.All);
		}

		internal void Stage(TreeItem item, AddFilesMode mode)
		{
			#region validate arguments

			if(item == null) throw new ArgumentNullException("item");
			if(item.Repository != Repository)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsNotHandledByThisRepository, "item"), "item");
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles(
					new AddFilesParameters(mode, item.RelativePath));
			}

			Refresh();
		}

		public void Stage(ICollection<TreeItem> items)
		{
			#region validate arguments

			if(items == null) throw new ArgumentNullException("item");
			if(items.Count == 0) return;

			#endregion

			var patterns = new string[items.Count];
			int id = 0;
			foreach(var item in items)
			{
				if(item == null) throw new ArgumentException(Resources.ExcCollectionMustNotContainNullElements, "items");
				if(item.Repository != Repository) throw new ArgumentException(string.Format(Resources.ExcAllObjectsMustBeHandledByThisRepository, "items"), "items");
				patterns[id++] = item.RelativePath;
			}

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles(
					new AddFilesParameters(AddFilesMode.All, patterns));
			}

			Refresh();
		}

		public void StageAll()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles(
					new AddFilesParameters(AddFilesMode.All, "."));
			}

			Refresh();
		}

		public void StageUpdated()
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				Repository.Accessor.AddFiles(
					new AddFilesParameters(AddFilesMode.Update, "."));
			}

			Refresh();
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
					Repository.Accessor.ResetFiles(
						new ResetFilesParameters("."));
				}
				else
				{
					Repository.Accessor.RemoveFiles(
						new RemoveFilesParameters(".")
						{
							Cached = true,
							Recursive = true,
						});
				}
			}

			Refresh();
		}

		internal void Unstage(TreeItem item)
		{
			#region validate arguments

			if(item == null) throw new ArgumentNullException("item");
			if(item.Repository != Repository)
			{
				throw new ArgumentException(string.Format(
					Resources.ExcSuppliedObjectIsNotHandledByThisRepository, "item"), "item");
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				if(!Repository.IsEmpty)
				{
					Repository.Accessor.ResetFiles(
						new ResetFilesParameters(item.RelativePath));
				}
				else
				{
					Repository.Accessor.RemoveFiles(
						new RemoveFilesParameters(item.RelativePath)
						{
							Cached = true,
							Recursive = true,
						});
				}
			}

			Refresh();
		}

		public void Unstage(ICollection<TreeItem> items)
		{
			#region validate arguments

			if(items == null) throw new ArgumentNullException("items");
			if(items.Count == 0) return;

			var patterns = new string[items.Count];
			int id = 0;
			foreach(var item in items)
			{
				if(item == null)
				{
					throw new ArgumentException(
						Resources.ExcCollectionMustNotContainNullElements, "items");
				}
				if(item.Repository != Repository)
				{
					throw new ArgumentException(string.Format(
						Resources.ExcAllObjectsMustBeHandledByThisRepository, "items"), "items");
				}
				patterns[id++] = item.RelativePath;
			}

			#endregion

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated))
			{
				if(!Repository.Head.IsEmpty)
				{
					Repository.Accessor.ResetFiles(
						new ResetFilesParameters(patterns));
				}
				else
				{
					Repository.Accessor.RemoveFiles(
						new RemoveFilesParameters(patterns)
						{
							Cached = true,
							Recursive = true,
						});
				}
			}

			Refresh();
		}

		#endregion

		#region reset

		/// <summary>Reset changes in working tree.</summary>
		/// <param name="mode">Reset mode</param>
		/// <exception cref="T:gitter.Git.GitException">git reset failed.</exception>
		public void Reset(ResetMode mode)
		{
			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				Repository.Accessor.Reset(
					new ResetParameters(mode));
			}

			Refresh();
		}

		#endregion

		#region clean

		/// <summary>Remove untracked and/or ignored files and (optionally) directories.</summary>
		/// <param name="includePattern">Files to clean.</param>
		/// <param name="excludePattern">Files to save.</param>
		/// <param name="mode">Clean mode.</param>
		/// <param name="removeDirectories"><c>true</c> to remove directories.</param>
		public void Clean(string includePattern, string excludePattern, CleanFilesMode mode, bool removeDirectories)
		{
			try
			{
				using(Repository.Monitor.BlockNotifications(
					RepositoryNotifications.IndexUpdated,
					RepositoryNotifications.WorktreeUpdated))
				{
					Repository.Accessor.CleanFiles(
						new CleanFilesParameters(includePattern)
						{
							ExcludePatterns = new[] { excludePattern },
							Force = true,
							Mode = mode,
							RemoveDirectories = removeDirectories,
						});
				}
			}
			finally
			{
				Refresh();
			}
		}

		#endregion

		#region apply

		/// <summary>Apply patch.</summary>
		/// <param name="patchSource">Patch source.</param>
		/// <param name="applyTo">Patch target.</param>
		/// <param name="reverse">Reverse patches.</param>
		public void ApplyPatch(IPatchSource patchSource, ApplyPatchTo applyTo, bool reverse = false)
		{
			if(patchSource == null) throw new ArgumentNullException("patchSource");

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.WorktreeUpdated))
			{
				using(var patch = patchSource.PreparePatchFile())
				{
					Repository.Accessor.ApplyPatch(
						new ApplyPatchParameters()
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
		public void ApplyPatches(IEnumerable<IPatchSource> patchSources, ApplyPatchTo applyTo, bool reverse = false)
		{
			if(patchSources == null) throw new ArgumentNullException("patchSources");

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
						Repository.Accessor.ApplyPatch(
							new ApplyPatchParameters()
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

		public Revision Commit(string message)
		{
			return Commit(message, false);
		}

		public Revision Commit(string message, bool amend)
		{
			#region validate arguments

			if(message == null) throw new ArgumentNullException("message");

			#endregion

			var currentBranch = Repository.Head.Pointer as Branch;

			using(Repository.Monitor.BlockNotifications(
				RepositoryNotifications.IndexUpdated,
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout))
			{
				var fileName = Path.Combine(
					Repository.GitDirectory,
					GitConstants.CommitMessageFileName);
				File.WriteAllText(fileName, message);
				bool commitSuccess = false;
				try
				{
					Repository.Accessor.Commit(
						new CommitParameters()
						{
							MessageFileName = Path.Combine(
								Repository.GitDirectory, GitConstants.CommitMessageFileName),
							Amend = amend,
						});
					commitSuccess = true;
				}
				finally
				{
					if(commitSuccess)
					{
						try
						{
							File.Delete(fileName);
						}
						catch { }
					}
				}
			}

			Revision commit = null;
			if(currentBranch != null)
			{
				var oldHeadRev = currentBranch.Revision;
				currentBranch.Refresh();
				commit = currentBranch.Revision;
				if(commit != oldHeadRev)
				{
					Repository.InvokeCommitCreated(commit);
				}
			}
			else
			{
				var oldHeadRev = Repository.Head.Revision;
				Repository.Head.Refresh();
				commit = Repository.Head.Revision;
				if(commit != oldHeadRev)
				{
					Repository.InvokeCommitCreated(commit);
				}
			}
			Repository.Head.NotifyRelogRecordAdded();
			Repository.InvokeStateChanged();
			Refresh();
			return commit;
		}

		public string LoadCommitMessage()
		{
			var fileName = Path.Combine(
				Repository.GitDirectory,
				GitConstants.CommitMessageFileName);
			try
			{
				if(File.Exists(fileName))
				{
					return File.ReadAllText(fileName);
				}
				else
				{
					return string.Empty;
				}
			}
			catch
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
			catch { }
		}

		#endregion

		#region diff

		public Diff GetStagedDiff()
		{
			return Repository.Accessor.QueryDiff(
				new QueryDiffParameters() { Cached = true });
		}

		public IAsyncFunc<Diff> GetStagedDiffAsync()
		{
			return AsyncFunc.Create(
				Repository.Accessor,
				(accessor, monitor) =>
				{
					return Repository.Accessor.QueryDiff(
						new QueryDiffParameters() { Cached = true });
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		public Diff GetStagedDiff(string path)
		{
			return Repository.Accessor.QueryDiff(
				new QueryDiffParameters() { Cached = true, Paths = new[] { path } });
		}

		public IAsyncFunc<Diff> GetStagedDiffAsync(string path)
		{
			return AsyncFunc.Create(
				Tuple.Create(Repository.Accessor, path),
				(data, monitor) =>
				{
					var accessor = data.Item1;
					var p = data.Item2;
					return Repository.Accessor.QueryDiff(
						new QueryDiffParameters() { Cached = true, Paths = new[] { p } });
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		public Diff GetUnstagedDiff()
		{
			return Repository.Accessor.QueryDiff(
				new QueryDiffParameters());
		}

		public IAsyncFunc<Diff> GetUnstagedDiffAsync(string path)
		{
			return AsyncFunc.Create(
				Tuple.Create(Repository.Accessor, path),
				(data, monitor) =>
				{
					var accessor = data.Item1;
					var p = data.Item2;
					return Repository.Accessor.QueryDiff(
						new QueryDiffParameters() { Cached = false, Paths = new[] { p } });
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		public IAsyncFunc<Diff> GetUnstagedDiffAsync()
		{
			return AsyncFunc.Create(
				Repository.Accessor,
				(accessor, monitor) =>
				{
					return Repository.Accessor.QueryDiff(
						new QueryDiffParameters() { Cached = false });
				},
				Resources.StrLoadingDiff.AddEllipsis(),
				"");
		}

		public Diff GetUnstagedDiff(string path)
		{
			return Repository.Accessor.QueryDiff(
				new QueryDiffParameters() { Paths = new[] { path } });
		}

		#endregion
	}
}
