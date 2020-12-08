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
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git repository.</summary>
	public sealed class Repository : IGitRepository
	{
		#region Data

		private RepositoryState _state;
		private User _userIdentity;

		#endregion

		#region Events

		/// <summary>Repository was updated.</summary>
		public event EventHandler Updated;

		/// <summary>State changed.</summary>
		public event EventHandler StateChanged;

		/// <summary>User identity changed.</summary>
		public event EventHandler UserIdentityChanged;

		/// <summary>Commit created at the top of HEAD.</summary>
		public event EventHandler<RevisionEventArgs> CommitCreated;

		/// <summary>Repository deleted.</summary>
		public event EventHandler Deleted;

		internal void OnStateChanged()
		{
			var state = GetState();
			if(_state != state)
			{
				_state = state;
				StateChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public void OnUserIdentityChanged() => UpdateUserIdentity(true);

		internal void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

		internal void OnCommitCreated(Revision revision) => CommitCreated?.Invoke(this, new RevisionEventArgs(revision));

		internal void OnDeleted() => Deleted?.Invoke(this, EventArgs.Empty);

		#endregion

		#region Static

		private static void ReportLoadProgress(IProgress<OperationProgress> progress, int val, string action)
		{
			if(progress != null)
			{
				var status = new OperationProgress
				{
					ActionName      = action,
					MaxProgress     = 8,
					CurrentProgress = val,
				};
				progress.Report(status);
			}
		}

		private static void LoadCore(Repository repository, IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			ReportLoadProgress(progress, 0, Resources.StrLoadingConfiguration.AddEllipsis());

			repository.Configuration.Refresh();

			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 1, Resources.StrLoadingReferences.AddEllipsis());

			var refs = repository.Accessor.QueryReferences.Invoke(
				new QueryReferencesParameters(ReferenceType.Branch | ReferenceType.Tag | ReferenceType.Stash));
			repository.Refs.Load(refs);

			if(refs.Stash != null)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ReportLoadProgress(progress, 2, Resources.StrLoadingStash.AddEllipsis());
				repository.Stash.Refresh();
			}

			repository.Notes.Refresh();

			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 3, Resources.StrLoadingHEAD.AddEllipsis());
			repository.Head = new Head(repository);

			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 4, Resources.StrLoadingRemotes.AddEllipsis());
			repository.Remotes.Refresh();


			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 5, Resources.StrLoadingSubmodules.AddEllipsis());
			repository.Submodules.Refresh();

			if(!repository.Head.IsEmpty)
			{
				cancellationToken.ThrowIfCancellationRequested();
				ReportLoadProgress(progress, 6, Resources.StrLoadingUsers.AddEllipsis());
				repository.Users.Refresh();
			}

			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 7, Resources.StrLoadingStatus.AddEllipsis());
			repository.Status.Refresh();

			cancellationToken.ThrowIfCancellationRequested();
			repository.UpdateState();

			cancellationToken.ThrowIfCancellationRequested();
			repository.UpdateUserIdentity(false);

			cancellationToken.ThrowIfCancellationRequested();
			repository.Monitor = new RepositoryMonitor(repository);
			repository.Monitor.IsEnabled = true;

			cancellationToken.ThrowIfCancellationRequested();
			ReportLoadProgress(progress, 8, Resources.StrCompleted.AddPeriod());
		}

		public static Repository Load(IGitAccessor gitAccessor, string workingDirectory)
		{
			return new Repository(gitAccessor, workingDirectory, true);
		}

		public static Task<Repository> LoadAsync(IGitAccessor gitAccessor, string workingDirectory,
			IProgress<OperationProgress> progress = default, CancellationToken cancellationToken = default)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNotNull(workingDirectory, nameof(workingDirectory));

			return Task.Run(
				() =>
				{
					progress?.Report(new OperationProgress(Resources.StrLoadingRepository.AddEllipsis()));
					var repository = new Repository(gitAccessor, workingDirectory, load: false);
					try
					{
						LoadCore(repository, progress, cancellationToken);
					}
					catch
					{
						repository.Dispose();
						throw;
					}
					return repository;
				});
		}

		private static string GetWorkingDirectory(string workingDirectory)
		{
			workingDirectory = Path.GetFullPath(workingDirectory);
			if(workingDirectory.Length > 3 && workingDirectory.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
			{
				workingDirectory = workingDirectory.Substring(0, workingDirectory.Length - 1);
			}
			return workingDirectory;
		}

		private static string GetGitDirectory(string workingDirectory)
		{
			const string GitDirPrefix = "gitdir: ";

			var gitDirectory = Path.Combine(workingDirectory, GitConstants.GitDir);
			if(!Directory.Exists(gitDirectory))
			{
				using var sr = new StreamReader(gitDirectory);
				string line = sr.ReadLine();
				while(line != null)
				{
					if(line.StartsWith(GitDirPrefix))
					{
						gitDirectory = line.Substring(GitDirPrefix.Length);
						if(!Path.IsPathRooted(gitDirectory))
						{
							gitDirectory = Path.GetFullPath(Path.Combine(
								workingDirectory,
								gitDirectory));
						}
						break;
					}
					line = sr.ReadLine();
				}
			}
			return gitDirectory;
		}

		private static ConfigurationManager GetConfigurationManager(string gitDirectory)
		{
			var configurationManager = default(ConfigurationManager);
			var cfgFileName = Path.Combine(gitDirectory, "gitter-config");
			try
			{
				if(File.Exists(cfgFileName))
				{
					using var fs = new FileStream(cfgFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
					configurationManager = new ConfigurationManager(new XmlAdapter(fs));
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
			return configurationManager ?? new ConfigurationManager("Gitter");
		}

		#endregion

		#region .ctor & finalizer

		/// <summary>Create <see cref="Repository"/>.</summary>
		/// <param name="gitAccessor">Git repository access provider.</param>
		/// <param name="workingDirectory">Repository working directory.</param>
		/// <param name="load"><c>true</c> to load repository; <c>false</c> otherwise.</param>
		private Repository(IGitAccessor gitAccessor, string workingDirectory, bool load)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNotNull(workingDirectory, nameof(workingDirectory));

			WorkingDirectory     = GetWorkingDirectory(workingDirectory);
			GitDirectory         = GetGitDirectory(WorkingDirectory);
			ConfigurationManager = GetConfigurationManager(GitDirectory);

			Accessor      = gitAccessor.CreateRepositoryAccessor(this);
			Revisions     = new RevisionCache(this);
			Configuration = new ConfigParametersCollection(this);
			Status        = new Status(this);
			Stash         = new StashedStatesCollection(this);
			Refs          = new RefsCollection(this);
			Notes         = new NotesCollection(this);
			Remotes       = new RemotesCollection(this);
			Submodules    = new SubmodulesCollection(this);
			Users         = new UsersCollection(this);
			Hooks         = new HooksCollection(this);

			if(load)
			{
				try
				{
					LoadCore(this);
				}
				catch
				{
					Dispose();
					throw;
				}
			}
		}

		/// <summary>Finalizes an instance of the <see cref="Repository"/> class.</summary>
		~Repository() => Dispose(disposing: false);

		#endregion

		/// <summary>Wrap <paramref name="revisionExpression"/> into a usable <see cref="IRevisionPointer"/>.</summary>
		/// <param name="revisionExpression">Valid revision expression.</param>
		/// <returns><see cref="IRevisionPointer"/> with <see cref="IRevisionPointer.Pointer"/> == <paramref name="revisionExpression"/>.</returns>
		public IRevisionPointer GetRevisionPointer(string revisionExpression)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(revisionExpression, nameof(revisionExpression));

			if(revisionExpression == GitConstants.HEAD)
			{
				return Head;
			}
			if(Hash.TryParse(revisionExpression, out var hash))
			{
				var revision = Revisions.TryGetRevision(hash);
				if(revision != null) return revision;
			}
			var reference = Refs.TryGetReference(revisionExpression);
			if(reference != null) return reference;

			return new DynamicRevisionPointer(this, revisionExpression);
		}

		#region Internal Services

		/// <summary>Returns repository monitor.</summary>
		/// <value>Repository monitor.</value>
		internal RepositoryMonitor Monitor { get; private set; }

		/// <summary>Returns repository monitor.</summary>
		/// <value>Repository monitor.</value>
		IRepositoryMonitor IGitRepository.Monitor => Monitor;

		#endregion

		#region Properties

		/// <summary>Returns object which provides raw access to this repository.</summary>
		/// <value>Object which provides raw access to this repository.</value>
		public IRepositoryAccessor Accessor { get; }

		/// <summary>Returns repository configuration manager.</summary>
		/// <value>Repository configuration manager.</value>
		public ConfigurationManager ConfigurationManager { get; }

		/// <summary>Returns repository configuration section.</summary>
		/// <value>Repository configuration section.</value>
		public Section ConfigSection => ConfigurationManager.RootSection;

		/// <summary>Returns repository working directory.</summary>
		/// <value>Repository working directory.</value>
		public string WorkingDirectory { get; }

		/// <summary>Returns repository directory (.git by default).</summary>
		/// <value>Repository directory.</value>
		public string GitDirectory { get; }

		/// <summary>Returns if it's an empty repository.</summary>
		/// <value><c>true</c>, if this is a an empty repository, <c>false</c> otherwise.</value>
		public bool IsEmpty => Head.IsEmpty && Refs.Heads.Count == 0;

		/// <summary>Returns if this repository is a shallow repository.</summary>
		/// <value><c>true</c>, if this is a shallow repository, <c>false</c> otherwise.</value>
		public bool IsShallow => File.Exists(GetGitFileName(GitConstants.ShallowFile));

		/// <summary>Returns revision cache.</summary>
		/// <value>Revision cache.</value>
		public RevisionCache Revisions { get; }

		/// <summary>Returns HEAD reference.</summary>
		/// <value>HEAD reference.</value>
		public Head Head { get; private set; }

		/// <summary>Returns references collection.</summary>
		/// <value>References collection.</value>
		public RefsCollection Refs { get; }

		/// <summary>Returns stash.</summary>
		/// <value>Stash.</value>
		public StashedStatesCollection Stash { get; }

		/// <summary>Returns notes collection.</summary>
		/// <value>Notes collection.</value>
		public NotesCollection Notes { get; }

		/// <summary>Returns remotes collection.</summary>
		/// <value>Remotes collection.</value>
		public RemotesCollection Remotes { get; }

		/// <summary>Returns submodules collection.</summary>
		/// <value>Submodules collection.</value>
		public SubmodulesCollection Submodules { get; }

		/// <summary>Returns working directory status.</summary>
		/// <value>Working directory status.</value>
		public Status Status { get; }

		/// <summary>Returns repository configuration.</summary>
		/// <value>Repository configuration.</value>
		public ConfigParametersCollection Configuration { get; }

		/// <summary>Returns hooks collection.</summary>
		/// <value>Hooks collection.</value>
		public HooksCollection Hooks { get; }

		/// <summary>Returns users collection.</summary>
		/// <value>Users collection.</value>
		public UsersCollection Users { get; }

		/// <summary>Returns repository state.</summary>
		/// <value>Repository state.</value>
		public RepositoryState State
		{
			get
			{
				UpdateState();
				return _state;
			}
		}

		#endregion

		private IRevisionPointer RevisionPointerFromGitFile(string fileName)
		{
			try
			{
				fileName = GetGitFileName(fileName);
				if(File.Exists(fileName))
				{
					using var sr = new StreamReader(fileName);
					return GetRevisionPointer(sr.ReadLine());
				}
			}
			catch(Exception exc) when(!exc.IsCritical())
			{
			}
			return default;
		}

		public IRevisionPointer MergeHead => RevisionPointerFromGitFile(GitConstants.MERGE_HEAD);

		public IRevisionPointer CherryPickHead => RevisionPointerFromGitFile(GitConstants.CHERRY_PICK_HEAD);

		public IRevisionPointer RevertHead => RevisionPointerFromGitFile(GitConstants.REVERT_HEAD);

		public IRevisionPointer RebaseHead => RevisionPointerFromGitFile("rebase-merge/head-name");

		private RepositoryState GetState()
		{
			var state = RepositoryState.Normal;
			if(File.Exists(GetGitFileName(GitConstants.MERGE_HEAD)))
			{
				state = RepositoryState.Merging;
			}
			else if(File.Exists(GetGitFileName(GitConstants.CHERRY_PICK_HEAD)))
			{
				state = RepositoryState.CherryPicking;
			}
			else if(File.Exists(GetGitFileName(GitConstants.REVERT_HEAD)))
			{
				state = RepositoryState.Reverting;
			}
			else if(Directory.Exists(GetGitFileName("rebase-apply")) || Directory.Exists(GetGitFileName("rebase-merge")))
			{
				state = RepositoryState.Rebasing;
			}
			return state;
		}

		private void UpdateState()
		{
			var state = GetState();
			if(_state != state)
			{
				_state = state;
				StateChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private void UpdateUserIdentity(bool raiseEvent)
		{
			User userIdentity;
			var name  = Configuration.TryGetParameterValue(GitConstants.UserNameParameter);
			var email = Configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
			if(name == null || email == null)
			{
				userIdentity = null;
			}
			else
			{
				if(_userIdentity == null || _userIdentity.Name != name || _userIdentity.Email != email)
				{
					userIdentity = Users.TryGetUser(name, email);
					if(userIdentity == null)
					{
						userIdentity = new User(this, name, email, 0);
					}
				}
				else
				{
					userIdentity = _userIdentity;
				}
			}
			if(userIdentity != _userIdentity)
			{
				_userIdentity = userIdentity;
				if(raiseEvent)
				{
					UserIdentityChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>Returns user identity.</summary>
		/// <value>User identity.</value>
		public User UserIdentity
		{
			get
			{
				UpdateUserIdentity(false);
				return _userIdentity;
			}
		}

		/// <summary>Returns full name for a file in repository directory (.git directory by default).</summary>
		/// <param name="file">File name.</param>
		/// <returns>Full name for a file in repository directory.</returns>
		public string GetGitFileName(string file)
		{
			return Path.Combine(GitDirectory, file);
		}

		/// <summary>Returns content of a file in repository directory (.git directory by default).</summary>
		/// <param name="file">File name.</param>
		/// <returns>Content of a file in repository directory.</returns>
		public string ReadGitFileContent(string file)
		{
			var fileName = GetGitFileName(file);
			return File.ReadAllText(fileName);
		}

		#region init

		public static void Init(IGitAccessor gitAccessor, string path, string template, bool bare)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNeitherNullNorWhitespace(path, nameof(path));

			gitAccessor.InitRepository.Invoke(new InitRepositoryParameters(path, template, bare));
		}

		public static void Init(IGitAccessor gitAccessor, string path, string template)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNeitherNullNorWhitespace(path, nameof(path));

			gitAccessor.InitRepository.Invoke(new InitRepositoryParameters(path, template, false));
		}

		public static void Init(IGitAccessor gitAccessor, string path, bool bare)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNeitherNullNorWhitespace(path, nameof(path));

			gitAccessor.InitRepository.Invoke(new InitRepositoryParameters(path, null, bare));
		}

		public static void Init(IGitAccessor gitAccessor, string path)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));
			Verify.Argument.IsNeitherNullNorWhitespace(path, nameof(path));

			gitAccessor.InitRepository.Invoke(new InitRepositoryParameters(path, null, false));
		}

		#endregion

		#region clone

		public static void Clone(
			IGitAccessor gitAccessor,
			string url, string path, string template, string remoteName,
			bool shallow, int depth, bool bare, bool mirror, bool recursive, bool noCheckout)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));

			gitAccessor.CloneRepository.Invoke(
				new CloneRepositoryParameters()
				{
					Url = url,
					Path = path,
					Template = template,
					RemoteName = remoteName,

					Shallow = shallow,
					Depth = depth,
					Bare = bare,
					Mirror = mirror,
					Recursive = recursive,
					NoCheckout = noCheckout,
				});
		}

		public static Task CloneAsync(
			IGitAccessor gitAccessor,
			string url, string path, string template, string remoteName,
			bool shallow, int depth, bool bare, bool mirror, bool recursive, bool noCheckout,
			IProgress<OperationProgress> progress, CancellationToken cancellationToken)
		{
			Verify.Argument.IsNotNull(gitAccessor, nameof(gitAccessor));

			return gitAccessor.CloneRepository.InvokeAsync(
				new CloneRepositoryParameters()
				{
					Url = url,
					Path = path,
					Template = template,
					RemoteName = remoteName,

					Shallow = shallow,
					Depth = depth,
					Bare = bare,
					Mirror = mirror,
					Recursive = recursive,
					NoCheckout = noCheckout,
				},
				progress, cancellationToken);
		}

		#endregion

		#region cherry-pick

		public async Task CherryPickAsync(CherryPickControl control, IProgress<OperationProgress> progress)
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			switch(control)
			{
				case CherryPickControl.Abort:
					progress?.Report(new OperationProgress(Resources.StrsAbortingCherryPick.AddEllipsis()));
					break;
				case CherryPickControl.Continue:
					progress?.Report(new OperationProgress(Resources.StrsContinuingCherryPick.AddEllipsis()));
					break;
				case CherryPickControl.Quit:
					progress?.Report(new OperationProgress(Resources.StrsQuitingCherryPick.AddEllipsis()));
					break;
				default:
					throw new ArgumentException(
						"Unknown CherryPickControl value: {0}".UseAsFormat(control),
						nameof(control));
			}

			var block = Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			try
			{
				await Accessor.CherryPick
					.InvokeAsync(new CherryPickParameters(control), progress, CancellationToken.None)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			catch
			{
				throw;
			}
			finally
			{
				block.Dispose();
				if(Head.Pointer is Branch branch && !branch.IsRemote)
				{
					branch.Refresh();
				}
				else
				{
					Head.Refresh();
				}
				Status.Refresh();
				OnStateChanged();
				OnUpdated();
			}
		}

		#endregion

		#region revert

		public async Task RevertAsync(RevertControl control, IProgress<OperationProgress> progress)
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			switch(control)
			{
				case RevertControl.Abort:
					progress?.Report(new OperationProgress(Resources.StrsAbortingRevert.AddEllipsis()));
					break;
				case RevertControl.Continue:
					progress?.Report(new OperationProgress(Resources.StrsContinuingRevert.AddEllipsis()));
					break;
				case RevertControl.Quit:
					progress?.Report(new OperationProgress(Resources.StrsQuitingRevert.AddEllipsis()));
					break;
				default:
					throw new ArgumentException(
						"Unknown RevertControl value: {0}".UseAsFormat(control),
						nameof(control));
			}

			var block = Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			try
			{
				await Accessor
					.Revert
					.InvokeAsync(new RevertParameters(control), progress, CancellationToken.None)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				block.Dispose();
				if(Head.Pointer is Branch branch && !branch.IsRemote)
				{
					branch.Refresh();
				}
				else
				{
					Head.Refresh();
				}
				Status.Refresh();
				OnStateChanged();
				OnUpdated();
			}
		}

		#endregion

		#region rebase

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Type of operation.</param>
		public void Rebase(RebaseControl control)
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			using(Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				try
				{
					Accessor.Rebase.Invoke(new RebaseParameters(control));
				}
				finally
				{
					Refs.RefreshBranches();
					Head.Refresh();
					if(Head.Pointer is Branch branch && !branch.IsRemote)
					{
						branch.Refresh();
					}
					Status.Refresh();
					OnStateChanged();
					OnUpdated();
				}
			}
		}

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Type of operation.</param>
		public async Task RebaseAsync(RebaseControl control, IProgress<OperationProgress> progress)
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			switch(control)
			{
				case RebaseControl.Abort:
					progress?.Report(new OperationProgress(Resources.StrsAbortingRebase.AddEllipsis()));
					break;
				case RebaseControl.Continue:
					progress?.Report(new OperationProgress(Resources.StrsContinuingRebase.AddEllipsis()));
					break;
				case RebaseControl.Skip:
					progress?.Report(new OperationProgress(Resources.StrsSkippingCommit.AddEllipsis()));
					break;
				default:
					throw new ArgumentException(
						"Unknown RebaseControl value: {0}".UseAsFormat(control),
						nameof(control));
			}

			var block = Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated);
			try
			{
				await Accessor
					.Rebase
					.InvokeAsync(new RebaseParameters(control), progress, CancellationToken.None);
			}
			finally
			{
				block.Dispose();
				Refs.RefreshBranches();
				Head.Refresh();
				if(Head.Pointer is Branch branch && !branch.IsRemote)
				{
					branch.Refresh();
				}
				Status.Refresh();
				OnStateChanged();
				OnUpdated();
			}
		}

		#endregion

		#region gc

		private static GarbageCollectParameters GetGarbageCollectParameters()
		{
			return new GarbageCollectParameters();
		}

		/// <summary>Perform garbage collection.</summary>
		public void GarbageCollect()
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			var parameters = GetGarbageCollectParameters();
			using(Monitor.BlockNotifications(RepositoryNotifications.BranchChanged))
			{
				Accessor.GarbageCollect.Invoke(parameters);
			}
		}

		/// <summary>Perform garbage collection.</summary>
		public Task GarbageCollectAsync(IProgress<OperationProgress> progress = default)
		{
			Verify.State.IsFalse(IsDisposed, "Repository is disposed.");

			progress?.Report(new OperationProgress(Resources.StrOptimizingRepository.AddEllipsis()));
			var parameters = GetGarbageCollectParameters();
			using(Monitor.BlockNotifications(RepositoryNotifications.BranchChanged))
			{
				return Accessor.GarbageCollect.InvokeAsync(parameters, progress, CancellationToken.None);
			}
		}

		#endregion

		#region IDisposable

		/// <summary>Gets a value indicating whether this instance is disposed.</summary>
		/// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed { get; private set; }

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			Monitor?.Dispose();
			(Accessor as IDisposable)?.Dispose();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if(!IsDisposed)
			{
				GC.SuppressFinalize(this);
				Dispose(true);
				IsDisposed = true;
			}
		}

		#endregion
	}
}
