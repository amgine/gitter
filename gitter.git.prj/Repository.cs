namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;

	using gitter.Framework;

	using gitter.Git.AccessLayer;
	using gitter.Framework.Configuration;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git repository.</summary>
	public sealed class Repository : IGitRepository
	{
		#region Data

		private readonly string _workingDirectory;
		private readonly string _gitDirectory;
		private readonly ConfigurationManager _configurationManager;

		private readonly IRepositoryAccessor _accessor;

		private readonly RevisionCache _revisionCache;
		private readonly ConfigParametersCollection _configuration;
		private readonly Status _status;
		private readonly StashedStatesCollection _stash;
		private readonly RefsCollection _refs;
		private readonly NotesCollection _notes;
		private readonly RemotesCollection _remotes;
		private readonly UsersCollection _users;
		private readonly SubmodulesCollection _submodules;
		private readonly HooksCollection _hooks;

		private Head _head;
		private RepositoryMonitor _monitor;
		private RepositoryState _state;
		private User _userIdentity;

		private bool _isDisposed;

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

		internal void InvokeStateChanged()
		{
			var state = GetState();
			if(_state != state)
			{
				_state = state;
				var handler = StateChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		public void InvokeUserIdentityChanged()
		{
			UpdateUserIdentity(true);
		}

		internal void InvokeUpdated()
		{
			var handler = Updated;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		internal void InvokeCommitCreated(Revision revision)
		{
			var handler = CommitCreated;
			if(handler != null) handler(this, new RevisionEventArgs(revision));
		}

		internal void InvokeDeleted()
		{
			var handler = Deleted;
			if(handler != null) handler(this, EventArgs.Empty);
		}

		#endregion

		#region Static

		private static void SetProgress(IAsyncProgressMonitor monitor, int val, string action)
		{
			if(monitor != null)
			{
				if(monitor.IsCancelRequested)
				{
					throw new OperationCanceledException();
				}
				monitor.SetProgress(val, action);
			}
		}

		private static void LoadCore(Repository repository, IAsyncProgressMonitor monitor)
		{
			if(monitor != null)
			{
				monitor.SetProgressRange(0, 8);
				monitor.SetProgress(0, Resources.StrLoadingConfiguration.AddEllipsis());
			}

			repository.Configuration.Refresh();

			SetProgress(monitor, 1, Resources.StrLoadingReferences.AddEllipsis());

			var refs = repository.Accessor.QueryReferences(
				new QueryReferencesParameters(ReferenceType.Branch | ReferenceType.Tag | ReferenceType.Stash));
			repository.Refs.Load(refs);

			if(refs.Stash != null)
			{
				SetProgress(monitor, 2, Resources.StrLoadingStash.AddEllipsis());
				repository.Stash.Refresh();
			}

			repository.Notes.Refresh();

			SetProgress(monitor, 3, Resources.StrLoadingHEAD.AddEllipsis());
			repository.Head = new Head(repository);

			SetProgress(monitor, 4, Resources.StrLoadingRemotes.AddEllipsis());
			repository.Remotes.Refresh();


			SetProgress(monitor, 5, Resources.StrLoadingSubmodules.AddEllipsis());
			repository.Submodules.Refresh();

			if(!repository.Head.IsEmpty)
			{
				SetProgress(monitor, 6, Resources.StrLoadingUsers.AddEllipsis());
				repository.Users.Refresh();
			}

			SetProgress(monitor, 7, Resources.StrLoadingStatus.AddEllipsis());
			repository.Status.Refresh();

			repository.Monitor = new RepositoryMonitor(repository);

			repository.UpdateState();

			repository.UpdateUserIdentity(false);

			SetProgress(monitor, 8, Resources.StrCompleted.AddPeriod());
		}

		public static Repository Load(IGitAccessor gitAccessor, string workingDirectory)
		{
			return new Repository(gitAccessor, workingDirectory, true);
		}

		public static IAsyncFunc<Repository> LoadAsync(IGitAccessor gitAccessor, string workingDirectory)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNotNull(workingDirectory, "workingDirectory");

			return AsyncFunc.Create(
				new
				{
					GitAccessor = gitAccessor,
					WorkingDirectory = workingDirectory,
				},
				(data, monitor) =>
				{
					var repository = new Repository(data.GitAccessor, data.WorkingDirectory, false);
					try
					{
						LoadCore(repository, monitor);
					}
					catch
					{
						repository.Dispose();
						throw;
					}
					return repository;
				},
				Resources.StrLoadingRepository.AddEllipsis(),
				string.Empty, true);
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
				using(var sr = new StreamReader(gitDirectory))
				{
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
			}
			return gitDirectory;
		}

		private static ConfigurationManager GetConfigurationManager(string gitDirectory)
		{
			ConfigurationManager configurationManager = null;
			var cfgFileName = Path.Combine(gitDirectory, "gitter-config");
			try
			{
				if(File.Exists(cfgFileName))
				{
					using(var fs = new FileStream(cfgFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						configurationManager = new ConfigurationManager(new XmlAdapter(fs));
					}
				}
			}
			catch
			{
			}
			if(configurationManager == null)
			{
				configurationManager = new ConfigurationManager("Gitter");
			}
			return configurationManager;
		}

		#endregion

		#region .ctor & finalizer

		/// <summary>Create <see cref="Repository"/>.</summary>
		/// <param name="gitAccessor">Git repository access provider.</param>
		/// <param name="workingDirectory">Repository working directory.</param>
		/// <param name="load"><c>true</c> to load repository; <c>false</c> otherwise.</param>
		private Repository(IGitAccessor gitAccessor, string workingDirectory, bool load)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNotNull(workingDirectory, "workingDirectory");

			_workingDirectory		= GetWorkingDirectory(workingDirectory);
			_gitDirectory			= GetGitDirectory(_workingDirectory);
			_configurationManager	= GetConfigurationManager(_gitDirectory);

			_accessor		= gitAccessor.CreateRepositoryAccessor(this);

			_revisionCache	= new RevisionCache(this);
			_configuration	= new ConfigParametersCollection(this);
			_status			= new Status(this);
			_stash			= new StashedStatesCollection(this);
			_refs			= new RefsCollection(this);
			_notes			= new NotesCollection(this);
			_remotes		= new RemotesCollection(this);
			_submodules		= new SubmodulesCollection(this);
			_users			= new UsersCollection(this);
			_hooks			= new HooksCollection(this);

			if(load)
			{
				try
				{
					LoadCore(this, null);
				}
				catch
				{
					Dispose();
					throw;
				}
			}
		}

		/// <summary>Finalizes an instance of the <see cref="Repository"/> class.</summary>
		~Repository()
		{
			Dispose(false);
		}

		#endregion

		/// <summary>Wrap <paramref name="revisionExpression"/> into a usable <see cref="IRevisionPointer"/>.</summary>
		/// <param name="revisionExpression">Valid revision expression.</param>
		/// <returns><see cref="IRevisionPointer"/> with <see cref="IRevisionPointer.Pointer"/> == <paramref name="revisionExpression"/>.</returns>
		public IRevisionPointer GetRevisionPointer(string revisionExpression)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(revisionExpression, "revisionExpression");

			if(revisionExpression == GitConstants.HEAD)
			{
				return _head;
			}
			if(GitUtils.IsValidSHA1(revisionExpression))
			{
				var revision = _revisionCache.TryGetRevision(revisionExpression);
				if(revision != null) return revision;
			}
			var reference = _refs.TryGetReference(revisionExpression);
			if(reference != null) return reference;

			return new DynamicRevisionPointer(this, revisionExpression);
		}

		#region Internal Services

		/// <summary>Returns repository monitor.</summary>
		/// <value>Repository monitor.</value>
		internal RepositoryMonitor Monitor
		{
			get { return _monitor; }
			private set { _monitor = value; }
		}

		#endregion

		#region Properties

		/// <summary>Returns object which provides raw access to this repository.</summary>
		/// <value>Object which provides raw access to this repository.</value>
		public IRepositoryAccessor Accessor
		{
			get { return _accessor; }
		}

		/// <summary>Returns repository configuration manager.</summary>
		/// <value>Repository configuration manager.</value>
		public ConfigurationManager ConfigurationManager
		{
			get { return _configurationManager; }
		}

		/// <summary>Returns repository configuration section.</summary>
		/// <value>Repository configuration section.</value>
		public Section ConfigSection
		{
			get { return _configurationManager.RootSection; }
		}

		/// <summary>Returns repository working directory.</summary>
		/// <value>Repository working directory.</value>
		public string WorkingDirectory
		{
			get { return _workingDirectory; }
		}

		/// <summary>Returns repository directory (.git by default).</summary>
		/// <value>Repository directory.</value>
		public string GitDirectory
		{
			get { return _gitDirectory; }
		}

		/// <summary>Returns if it's an empty repository.</summary>
		/// <value><c>true</c>, if this is a an empty repository, <c>false</c> otherwise.</value>
		public bool IsEmpty
		{
			get { return _head.IsEmpty && _refs.Heads.Count == 0; }
		}

		/// <summary>Returns if this repository is a shallow repository.</summary>
		/// <value><c>true</c>, if this is a shallow repository, <c>false</c> otherwise.</value>
		public bool IsShallow
		{
			get { return File.Exists(GetGitFileName(GitConstants.ShallowFile)); }
		}

		/// <summary>Returns revision cache.</summary>
		/// <value>Revision cache.</value>
		public RevisionCache Revisions
		{
			get { return _revisionCache; }
		}

		/// <summary>Returns HEAD reference.</summary>
		/// <value>HEAD reference.</value>
		public Head Head
		{
			get { return _head; }
			private set { _head = value; }
		}

		/// <summary>Returns references collection.</summary>
		/// <value>References collection.</value>
		public RefsCollection Refs
		{
			get { return _refs; }
		}

		/// <summary>Returns stash.</summary>
		/// <value>Stash.</value>
		public StashedStatesCollection Stash
		{
			get { return _stash; }
		}

		/// <summary>Returns notes collection.</summary>
		/// <value>Notes collection.</value>
		public NotesCollection Notes
		{
			get { return _notes; }
		}

		/// <summary>Returns remotes collection.</summary>
		/// <value>Remotes collection.</value>
		public RemotesCollection Remotes
		{
			get { return _remotes; }
		}

		/// <summary>Returns submodules collection.</summary>
		/// <value>Submodules colection.</value>
		public SubmodulesCollection Submodules
		{
			get { return _submodules; }
		}

		/// <summary>Returns working directory status.</summary>
		/// <value>Working directory status.</value>
		public Status Status
		{
			get { return _status; }
		}

		/// <summary>Returns repository configuration.</summary>
		/// <value>Repository configuration.</value>
		public ConfigParametersCollection Configuration
		{
			get { return _configuration; }
		}

		/// <summary>Returns hooks collection.</summary>
		/// <value>Hooks collection.</value>
		public HooksCollection Hooks
		{
			get { return _hooks; }
		}

		/// <summary>Returns users collection.</summary>
		/// <value>Users collection.</value>
		public UsersCollection Users
		{
			get { return _users; }
		}

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
					using(var sr = new StreamReader(fileName))
					{
						return GetRevisionPointer(sr.ReadLine());
					}
				}
				else
				{
					return null;
				}
			}
			catch
			{
				return null;
			}
		}

		public IRevisionPointer MergeHead
		{
			get { return RevisionPointerFromGitFile(GitConstants.MERGE_HEAD); }
		}

		public IRevisionPointer CherryPickHead
		{
			get { return RevisionPointerFromGitFile(GitConstants.CHERRY_PICK_HEAD); }
		}

		public IRevisionPointer RevertHead
		{
			get { return RevisionPointerFromGitFile(GitConstants.REVERT_HEAD); }
		}

		public IRevisionPointer RebaseHead
		{
			get { return RevisionPointerFromGitFile("rebase-merge/head-name"); }
		}

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
			else if(Directory.Exists(GetGitFileName("rebase-apply")))
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
				var handler = StateChanged;
				if(handler != null) handler(this, EventArgs.Empty);
			}
		}

		private void UpdateUserIdentity(bool raiseEvent)
		{
			User userIdentity;
			var name	= _configuration.TryGetParameterValue(GitConstants.UserNameParameter);
			var email	= _configuration.TryGetParameterValue(GitConstants.UserEmailParameter);
			if(name == null || email == null)
			{
				userIdentity = null;
			}
			else
			{
				if(_userIdentity == null || _userIdentity.Name != name || _userIdentity.Email != email)
				{
					userIdentity = _users.TryGetUser(name, email);
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
					var handler = UserIdentityChanged;
					if(handler != null) handler(this, EventArgs.Empty);
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
			return Path.Combine(_gitDirectory, file);
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
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			gitAccessor.InitRepository(new InitRepositoryParameters(path, template, bare));
		}

		public static void Init(IGitAccessor gitAccessor, string path, string template)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			gitAccessor.InitRepository(new InitRepositoryParameters(path, template, false));
		}

		public static void Init(IGitAccessor gitAccessor, string path, bool bare)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			gitAccessor.InitRepository(new InitRepositoryParameters(path, null, bare));
		}

		public static void Init(IGitAccessor gitAccessor, string path)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");
			Verify.Argument.IsNeitherNullNorWhitespace(path, "path");

			gitAccessor.InitRepository(new InitRepositoryParameters(path, null, false));
		}

		#endregion

		#region clone

		public static void Clone(
			IGitAccessor gitAccessor,
			string url, string path, string template, string remoteName,
			bool shallow, int depth, bool bare, bool mirror, bool recursive, bool noCheckout)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");

			gitAccessor.CloneRepository(
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

		public static IAsyncAction CloneAsync(
			IGitAccessor gitAccessor,
			string url, string path, string template, string remoteName,
			bool shallow, int depth, bool bare, bool mirror, bool recursive, bool noCheckout)
		{
			Verify.Argument.IsNotNull(gitAccessor, "gitAccessor");

			return AsyncAction.Create(
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
				(parameters, monitor) =>
				{
					parameters.Monitor = monitor;
					gitAccessor.CloneRepository(parameters);
				},
				Resources.StrCloningRepository.AddEllipsis(),
				Resources.StrfCloning.UseAsFormat(url).AddEllipsis(),
				true);
		}

		#endregion

		#region rebase

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Type of operation.</param>
		public void Rebase(RebaseControl control)
		{
			using(Monitor.BlockNotifications(
				RepositoryNotifications.BranchChanged,
				RepositoryNotifications.Checkout,
				RepositoryNotifications.WorktreeUpdated,
				RepositoryNotifications.IndexUpdated))
			{
				try
				{
					_accessor.Rebase(control);
				}
				finally
				{
					_refs.RefreshBranches();
					_head.Refresh();
					_status.Refresh();
					InvokeStateChanged();
					InvokeUpdated();
				}
			}
		}

		/// <summary>Control rebase process.</summary>
		/// <param name="control">Type of operation.</param>
		public IAsyncAction RebaseAsync(RebaseControl control)
		{
			string details;
			switch(control)
			{
				case RebaseControl.Abort:
					details = Resources.StrsAbortingRebase.AddEllipsis();
					break;
				case RebaseControl.Continue:
					details = Resources.StrsContinuingRebase.AddEllipsis();
					break;
				case RebaseControl.Skip:
					details = Resources.StrsSkippingCommit.AddEllipsis();
					break;
				default:
					throw new ArgumentException(
						"Unknown RebaseControl value: {0}".UseAsFormat(control),
						"control");
			}
			return AsyncAction.Create(
				new
				{
					Repository = this,
					Control = control,
				},
				(data, monitor) =>
				{
					data.Repository.Rebase(data.Control);
				},
				Resources.StrRebase,
				details);
		}

		#endregion

		#region gc

		/// <summary>Perform garbage collection.</summary>
		public void GarbageCollect()
		{
			Accessor.GarbageCollect(new GarbageCollectParameters());
		}

		/// <summary>Perform garbage collection.</summary>
		public IAsyncAction GarbageCollectAsync()
		{
			return AsyncAction.Create(
				this,
				(repository, monitor) =>
				{
					repository.Accessor.GarbageCollect(
						new GarbageCollectParameters()
						{
							//Monitor = monitor,
						});
				},
				Resources.StrHousekeeping,
				Resources.StrOptimizingRepository.AddEllipsis());
		}

		#endregion

		#region IDisposable

		/// <summary>Gets a value indicating whether this instance is disposed.</summary>
		/// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		/// <summary>Releases unmanaged and - optionally - managed resources.</summary>
		/// <param name="disposing">
		/// <c>true</c> to release both managed and unmanaged resources;
		/// <c>false</c> to release only unmanaged resources.
		/// </param>
		private void Dispose(bool disposing)
		{
			if(Monitor != null)
			{
				Monitor.Shutdown();
			}
			var disposable = _accessor as IDisposable;
			if(disposable != null)
			{
				disposable.Dispose();
			}
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
