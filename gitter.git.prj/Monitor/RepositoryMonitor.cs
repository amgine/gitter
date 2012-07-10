//#define TRACE_FS_EVENTS

namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;

	/// <summary>Watches git repository and notifies about external changes.</summary>
	public sealed class RepositoryMonitor : IDisposable
	{
		private readonly Repository _repository;
		private readonly FileSystemWatcher _fswWorkDir;
		private readonly FileSystemWatcher _fswGitDir;
		#if TRACE_FS_EVENTS
		private readonly StreamWriter _sw;
		#endif

		private readonly List<NotificationBlock> _blockedNotifications;
		private readonly Queue<IRepositoryChangedNotification> _pendingNotifications;
		private readonly Queue<IRepositoryChangedNotification> _delayedNotifications;
		private readonly Queue<NotificationDelayedUnblock> _delayedUnblocks;
		private AutoResetEvent _evGotNotification;
		private AutoResetEvent _evGotDelayedNotification;
		private AutoResetEvent _evGotDelayedUnblock;
		private ManualResetEvent _evExit;
		private Thread _notificationThread;
		private Thread _delayedNotificationThread;
		private Thread _delayedUnblockingThread;
		private const int _notificationDelayTime = 750;
		private const int _unblockDelayTime = 750;

		private sealed class NotificationBlock
		{
			private readonly object _key;
			private readonly object _notificationType;

			public NotificationBlock(object key, object notificationType)
			{
				_key = key;
				_notificationType = notificationType;
			}

			public object Key
			{
				get { return _key;}
			}

			public object NotificationType
			{
				get { return _notificationType; }
			}
		}

		private sealed class NotificationDelayedUnblock
		{
			private readonly object _key;
			private readonly object _notificationType;
			private readonly DateTime _createdOn;

			public NotificationDelayedUnblock(DateTime createdOn, object key, object notificationType)
			{
				_createdOn = createdOn;
				_key = key;
				_notificationType = notificationType;
			}

			public DateTime CreatedOn
			{
				get { return _createdOn; }
			}

			public object Key
			{
				get { return _key; }
			}

			public object NotificationType
			{
				get { return _notificationType; }
			}
		}

		private sealed class NotificationsBlockToken : IDisposable
		{
			private readonly RepositoryMonitor _watcher;
			private readonly object[] _notifications;
			private readonly object _key;

			public NotificationsBlockToken(RepositoryMonitor watcher, object key, object[] notifications)
			{
				_watcher = watcher;
				_notifications = notifications;
				_key = key;
			}

			public void Dispose()
			{
				if(_notifications != null && _notifications.Length != 0)
				{
					_watcher.UnblockNotifications(_key, _notifications);
				}
			}
		}

		private enum ChangedPath
		{
			None,

			GitDir,
			WorkDir,
		}

		private enum GitSubdirectory
		{
			Unknown,

			Root,
			Refs,
			RefsHeads,
			RefsRemotes,
			RefsTags,
			RefsNotes,
		}

		/// <summary>Create <see cref="RepositoryMonitor"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal RepositoryMonitor(Repository repository)
		{
			if(repository == null) throw new ArgumentNullException("repository");

			_repository = repository;

			_blockedNotifications = new List<NotificationBlock>();
			_pendingNotifications = new Queue<IRepositoryChangedNotification>();
			_delayedNotifications = new Queue<IRepositoryChangedNotification>();
			_delayedUnblocks = new Queue<NotificationDelayedUnblock>();

			_fswGitDir = new FileSystemWatcher(repository.GitDirectory)
			{
				NotifyFilter =
					NotifyFilters.CreationTime |
					NotifyFilters.DirectoryName |
					NotifyFilters.FileName |
					NotifyFilters.LastWrite,
				IncludeSubdirectories = true,
			};
			_fswWorkDir = new FileSystemWatcher(repository.WorkingDirectory)
			{
				NotifyFilter =
					NotifyFilters.CreationTime |
					NotifyFilters.DirectoryName |
					NotifyFilters.FileName |
					NotifyFilters.LastWrite,
				IncludeSubdirectories = true,
			};

			_fswGitDir.Deleted += OnGitDirDeleted;
			_fswGitDir.Renamed += OnGitDirRenamed;

			_fswWorkDir.Created += OnWorkDirCreated;
			_fswWorkDir.Deleted += OnWorkDirDeleted;
			_fswWorkDir.Renamed += OnWorkDirRenamed;
			_fswWorkDir.Changed += OnWorkDirChanged;

			#if TRACE_FS_EVENTS
			_sw = new StreamWriter(@"F:\test.txt");
			#endif

			_evGotNotification = new AutoResetEvent(false);
			_evGotDelayedNotification = new AutoResetEvent(false);
			_evGotDelayedUnblock = new AutoResetEvent(false);
			_evExit = new ManualResetEvent(false);
			_notificationThread =
				new Thread(ApplyProc)
				{
					Name = "Repository watcher notification thread",
					IsBackground = true,
				};
			_notificationThread.Start();
			_delayedNotificationThread =
				new Thread(DelayProc)
				{
					Name = "Repository watcher delayed notification thread",
					IsBackground = true,
				};
			_delayedNotificationThread.Start();
			_delayedUnblockingThread =
				new Thread(DelayUnblockProc)
				{
					Name = "Repository watcher delayed unblocking thread",
					IsBackground = true,
				};
			_delayedUnblockingThread.Start();

			_fswGitDir.EnableRaisingEvents = true;
			_fswWorkDir.EnableRaisingEvents = true;
		}

		private void ApplyProc()
		{
			var wh = new WaitHandle[] { _evExit, _evGotNotification };
			while(true)
			{
				IRepositoryChangedNotification notification = null;
				lock(_pendingNotifications)
				{
					if(_pendingNotifications.Count != 0)
					{
						notification = _pendingNotifications.Dequeue();
					}
				}
				if(notification == null)
				{
					if(WaitHandle.WaitAny(wh) == 0) return;
					continue;
				}
				try
				{
					notification.Apply(_repository);
				}
				catch
				{
				}
			}
		}

		private void DelayProc()
		{
			var wh = new WaitHandle[] { _evExit, _evGotDelayedNotification };
			var notifications = new List<IRepositoryChangedNotification>();
			while(true)
			{
				lock(_delayedNotifications)
				{
					while(_delayedNotifications.Count != 0)
						notifications.Add(_delayedNotifications.Dequeue());
				}
				if(notifications.Count == 0)
				{
					if(WaitHandle.WaitAny(wh) == 0) return;
					continue;
				}
				switch(WaitHandle.WaitAny(wh, _notificationDelayTime))
				{
					case 0:
						return;
					case 1:
						continue;
					case WaitHandle.WaitTimeout:
						WorktreeUpdatedNotification globalwtn = null;
						int count = 0;
						foreach(var n in notifications)
						{
							var wtn = n as WorktreeUpdatedNotification;
							if(wtn != null)
							{
								if(count == 0)
								{
									globalwtn = wtn;
									++count;
								}
								else
								{
									globalwtn = new WorktreeUpdatedNotification("");
									++count;
								}
							}
							else
							{
								EmitNotification(n);
							}
						}
						notifications.Clear();
						if(globalwtn != null)
							EmitNotification(globalwtn);
						break;
				}
			}
		}

		private void DelayUnblockProc()
		{
			var wh = new WaitHandle[] { _evExit, _evGotDelayedUnblock };
			while(true)
			{
				NotificationDelayedUnblock unblock = null;
				lock(_delayedUnblocks)
				{
					if(_delayedUnblocks.Count != 0)
					{
						unblock = _delayedUnblocks.Dequeue();
					}
				}
				if(unblock == null)
				{
					if(WaitHandle.WaitAny(wh) == 0) return;
					continue;
				}
				var now = DateTime.UtcNow;
				var dt = _unblockDelayTime - (int)(now - unblock.CreatedOn).TotalMilliseconds;
				if(dt < 0) dt = 0;
				if(_evExit.WaitOne(dt)) return;
				UnblockNotification(unblock);
			}
		}

		private ChangedPath GetChangedPath(string path)
		{
			if(path == GitConstants.GitDir || path.StartsWith(GitConstants.GitDir + Path.DirectorySeparatorChar))
			{
				return ChangedPath.GitDir;
			}
			else
			{
				return ChangedPath.WorkDir;
			}
		}

		private static bool CheckValue(string path, string test, int pos)
		{
			if(path.Length < test.Length + pos) return false;
			return path.IndexOf(test, pos, test.Length) != -1;
		}

		private GitSubdirectory GetSubdirectory(string path)
		{
			if(path.IndexOf(Path.DirectorySeparatorChar) == -1)
			{
				return GitSubdirectory.Root;
			}
			if(CheckValue(path, "refs" + Path.DirectorySeparatorChar, 0))
			{
				if(path.Length <= 5 + 5)
				{
					return GitSubdirectory.Refs;
				}
				if(CheckValue(path, "heads" + Path.DirectorySeparatorChar, 0 + 5))
				{
					return GitSubdirectory.RefsHeads;
				}
				if(CheckValue(path, "remotes" + Path.DirectorySeparatorChar, 0 + 5))
				{
					return GitSubdirectory.RefsRemotes;
				}
				if(CheckValue(path, "tags" + Path.DirectorySeparatorChar, 0 + 5))
				{
					return GitSubdirectory.RefsTags;
				}
				return GitSubdirectory.Unknown;
			}
			return GitSubdirectory.Unknown;
		}

		private void EmitNotification(IRepositoryChangedNotification notification)
		{
			lock(_pendingNotifications)
			{
				_pendingNotifications.Enqueue(notification);
			}
			_evGotNotification.Set();
		}

		private void EmitDelayedNotification(IRepositoryChangedNotification notification)
		{
			lock(_delayedNotifications)
			{
				_delayedNotifications.Enqueue(notification);
			}
			_evGotDelayedNotification.Set();
		}

		private void OnWorkDirCreated(object sender, FileSystemEventArgs e)
		{
			#if TRACE_FS_EVENTS
			lock(_sw)
			{
				_sw.WriteLine(string.Format("{0} {1}", e.ChangeType, e.Name));
				_sw.Flush();
			}
			#endif
			switch(GetChangedPath(e.Name))
			{
				case ChangedPath.WorkDir:
					if(!IsBlocked(RepositoryNotifications.WorktreeUpdated))
					{
						EmitDelayedNotification(new WorktreeUpdatedNotification(e.Name));
					}
					break;
			}
		}

		private void OnWorkDirChanged(object sender, FileSystemEventArgs e)
		{
			switch(GetChangedPath(e.Name))
			{
				case ChangedPath.WorkDir:
					if(e.Name == GitConstants.SubmodulesConfigFile)
					{
						if(!IsBlocked(RepositoryNotifications.SubmodulesChanged))
						{
							EmitNotification(new SubmodulesChangedNotification());
						}
					}
					if(!IsBlocked(RepositoryNotifications.WorktreeUpdated))
					{
						EmitDelayedNotification(new WorktreeUpdatedNotification(e.Name));
					}
					break;
			}
		}

		private void OnGitDirDeleted(object sender, FileSystemEventArgs e)
		{
			switch(GetSubdirectory(e.Name))
			{
				case GitSubdirectory.Refs:
					if(e.Name == @"refs\stash" + GitConstants.LockPostfix)
					{
#if TRACE_FS_EVENTS
						lock(_sw)
						{
							_sw.WriteLine("Detected possible stash change");
							_sw.Flush();
						}
#endif
						if(!IsBlocked(RepositoryNotifications.StashChanged))
						{
							EmitNotification(new StashChangedNotification());
						}
					}
					break;
				case GitSubdirectory.RefsHeads:
					if(e.Name.EndsWith(GitConstants.LockPostfix))
					{
						int pos = GitConstants.LocalBranchPrefix.Length;
						var name = e.Name.Substring(pos, e.Name.Length - pos - GitConstants.LockPostfix.Length)
										 .Replace(Path.DirectorySeparatorChar, '/');
#if TRACE_FS_EVENTS
						lock(_sw)
						{
							_sw.WriteLine(string.Format("Detected possible branch change: {0}", name));
							_sw.Flush();
						}
#endif
						if(!IsBlocked(RepositoryNotifications.BranchChanged))
						{
							EmitNotification(new BranchChangedNotification(name, false));
						}
					}
					break;
				case GitSubdirectory.RefsRemotes:
					if(e.Name.EndsWith(GitConstants.LockPostfix))
					{
						int pos = GitConstants.RemoteBranchPrefix.Length;
						var name = e.Name.Substring(pos, e.Name.Length - pos - GitConstants.LockPostfix.Length)
										 .Replace(Path.DirectorySeparatorChar, '/');
#if TRACE_FS_EVENTS
						lock(_sw)
						{
							_sw.WriteLine(string.Format("Detected possible remote branch change: {0}", name));
							_sw.Flush();
						}
#endif
						if(!IsBlocked(RepositoryNotifications.BranchChanged))
						{
							EmitNotification(new BranchChangedNotification(name, true));
						}
					}
					break;
				case GitSubdirectory.RefsTags:
					if(e.Name.EndsWith(GitConstants.LockPostfix))
					{
						var tagFileName = e.FullPath.Substring(0, e.FullPath.Length - GitConstants.LockPostfix.Length);
						if(!File.Exists(tagFileName))
						{
							var pos = GitConstants.TagPrefix.Length;
							var name = e.Name.Substring(pos, e.Name.Length - pos - GitConstants.LockPostfix.Length)
											 .Replace(Path.DirectorySeparatorChar, '/');
#if TRACE_FS_EVENTS
							lock(_sw)
							{
								_sw.WriteLine(string.Format("Detected possible tag removal: {0}", name));
								_sw.Flush();
							}
#endif
							if(!IsBlocked(RepositoryNotifications.TagChanged))
							{
								EmitNotification(new TagChangedNotification(name));
							}
						}
					}
					break;
				case GitSubdirectory.Root:
					{
						if(e.Name.Length == 0)
						{
#if TRACE_FS_EVENTS
							lock(_sw)
							{
								_sw.WriteLine("Repository destruction detected");
								_sw.Flush();
							}
#endif
							EmitNotification(new RepositoryRemovedNotification());
							IsEnabled = false;
						}
						else
						{
							switch(e.Name)
							{
								case GitConstants.HEAD + GitConstants.LockPostfix:
#if TRACE_FS_EVENTS
									lock(_sw)
									{
										_sw.WriteLine("Checkout detected");
										_sw.Flush();
									}
#endif
									if(!IsBlocked(RepositoryNotifications.Checkout))
									{
										EmitNotification(new CheckoutNotification());
									}
									break;
								case "config" + GitConstants.LockPostfix:
#if TRACE_FS_EVENTS
									lock(_sw)
									{
										_sw.WriteLine("Config update detected");
										_sw.Flush();
									}
#endif
									if(!IsBlocked(RepositoryNotifications.ConfigUpdated))
									{
										EmitNotification(new ConfigUpdatedNotification());
									}
									break;
								case "index" + GitConstants.LockPostfix:
#if TRACE_FS_EVENTS
									lock(_sw)
									{
										_sw.WriteLine("Index update detected");
										_sw.Flush();
									}
#endif
									if(!IsBlocked(RepositoryNotifications.IndexUpdated))
									{
										EmitNotification(new IndexUpdatedNotification());
									}
									break;
							}
						}
					}
					break;
			}
		}

		private void OnWorkDirDeleted(object sender, FileSystemEventArgs e)
		{
			#if TRACE_FS_EVENTS
			lock(_sw)
			{
				_sw.WriteLine(string.Format("{0} {1}", e.ChangeType, e.Name));
				_sw.Flush();
			}
			#endif
			switch(GetChangedPath(e.Name))
			{
				case ChangedPath.WorkDir:
					if(e.Name == GitConstants.SubmodulesConfigFile)
					{
						if(!IsBlocked(RepositoryNotifications.SubmodulesChanged))
						{
							EmitNotification(new SubmodulesChangedNotification());
						}
					}
					if(!IsBlocked(RepositoryNotifications.WorktreeUpdated))
					{
						EmitDelayedNotification(new WorktreeUpdatedNotification(e.Name));
					}
					break;
			}
		}

		private void OnGitDirRenamed(object sender, RenamedEventArgs e)
		{
			switch(GetSubdirectory(e.OldName))
			{
				case GitSubdirectory.RefsHeads:
					{
						int pos = GitConstants.LocalBranchPrefix.Length;
						var oldName = e.OldName.Substring(pos);
						var newName = e.Name.Substring(pos);
						if(newName + GitConstants.LockPostfix == oldName)
						{
							newName = newName.Replace(Path.DirectorySeparatorChar, '/');
							#if TRACE_FS_EVENTS
							lock(_sw)
							{
								_sw.WriteLine("Detected branch creation: " + newName);
								_sw.Flush();
							}
							#endif
							if(!IsBlocked(RepositoryNotifications.BranchChanged))
							{
								EmitNotification(new BranchChangedNotification(newName, false));
							}
						}
					}
					break;
				case GitSubdirectory.RefsRemotes:
					{
						int pos = GitConstants.RemoteBranchPrefix.Length;
						var oldName = e.OldName.Substring(pos);
						var newName = e.Name.Substring(pos);
						if(newName + GitConstants.LockPostfix == oldName)
						{
							newName = newName.Replace(Path.DirectorySeparatorChar, '/');
							#if TRACE_FS_EVENTS
							lock(_sw)
							{
								_sw.WriteLine("Detected remote branch creation: " + newName);
								_sw.Flush();
							}
							#endif
							if(!IsBlocked(RepositoryNotifications.BranchChanged))
							{
								EmitNotification(new BranchChangedNotification(newName, true));
							}
						}
					}
					break;
				case GitSubdirectory.RefsTags:
					{
						int pos = GitConstants.TagPrefix.Length;
						var oldName = e.OldName.Substring(pos);
						var newName = e.Name.Substring(pos);
						if(newName + GitConstants.LockPostfix == oldName)
						{
							newName = newName.Replace(Path.DirectorySeparatorChar, '/');
							#if TRACE_FS_EVENTS
							lock(_sw)
							{
								_sw.WriteLine("Detected tag creation: " + newName);
								_sw.Flush();
							}
							#endif
							if(!IsBlocked(RepositoryNotifications.TagChanged))
							{
								EmitNotification(new TagChangedNotification(newName));
							}
						}
					}
					break;
			}
		}

		private void OnWorkDirRenamed(object sender, RenamedEventArgs e)
		{
			#if TRACE_FS_EVENTS
			lock(_sw)
			{
				_sw.WriteLine(string.Format("{0} {1} -> {2}", e.ChangeType, e.OldName, e.Name));
				_sw.Flush();
			}
			#endif
			switch(GetChangedPath(e.OldName))
			{
				case ChangedPath.WorkDir:
					#if TRACE_FS_EVENTS
					lock(_sw)
					{
						_sw.WriteLine("Detected working directory file rename: " + e.OldName + " -> " + e.Name);
						_sw.Flush();
					}
					#endif
					if(e.OldName == GitConstants.SubmodulesConfigFile || e.Name == GitConstants.SubmodulesConfigFile)
					{
						if(!IsBlocked(RepositoryNotifications.SubmodulesChanged))
						{
							EmitNotification(new SubmodulesChangedNotification());
						}
					}
					if(!IsBlocked(RepositoryNotifications.WorktreeUpdated))
					{
						EmitDelayedNotification(new WorktreeUpdatedNotification(e.OldName));
						EmitDelayedNotification(new WorktreeUpdatedNotification(e.Name));
					}
					break;
			}
		}

		public bool IsEnabled
		{
			get { return _fswWorkDir.EnableRaisingEvents; }
			set { _fswWorkDir.EnableRaisingEvents = value; }
		}

		private bool IsBlocked(object notificationType)
		{
			bool blocked = false;
			lock(_blockedNotifications)
			{
				foreach(var block in _blockedNotifications)
				{
					if(block.NotificationType == notificationType)
					{
						blocked = true;
						break;
					}
				}
			}
			return blocked;
		}

		private void UnblockNotification(NotificationDelayedUnblock unblock)
		{
			lock(_blockedNotifications)
			{
				for(int i = _blockedNotifications.Count - 1; i >= 0; --i)
				{
					var notification = _blockedNotifications[i];
					if ((notification.Key == unblock.Key) &&
						(notification.NotificationType == unblock.NotificationType))
					{
						_blockedNotifications.RemoveAt(i);
						break;
					}
				}
			}
		}

		public IDisposable BlockNotifications(params object[] notifications)
		{
			var key = new object();
			lock(_blockedNotifications)
			{
				foreach(var notification in notifications)
				{
					_blockedNotifications.Add(new NotificationBlock(key, notification));
				}
			}
			return new NotificationsBlockToken(this, key, notifications);
		}

		private void UnblockNotifications(object key, IEnumerable<object> notifications)
		{
			lock(_delayedUnblocks)
			{
				var time = DateTime.UtcNow;
				bool hadAny = false;
				foreach(var notification in notifications)
				{
					_delayedUnblocks.Enqueue(new NotificationDelayedUnblock(time, key, notification));
					hadAny = true;
				}
				if(hadAny)
				{
					_evGotDelayedUnblock.Set();
				}
			}
		}

		public void Shutdown()
		{
			_fswWorkDir.EnableRaisingEvents = false;
			_fswWorkDir.Dispose();
			_fswGitDir.EnableRaisingEvents = false;
			_fswGitDir.Dispose();
			_evExit.Set();
			_notificationThread.Join();
			_delayedNotificationThread.Join();
			_delayedUnblockingThread.Join();
			_pendingNotifications.Clear();
			_delayedNotifications.Clear();
			#if TRACE_FS_EVENTS
			_sw.Close();
			#endif
		}

		#region IDisposable Members

		public void Dispose()
		{
			_fswWorkDir.Dispose();
			_fswGitDir.Dispose();
			_evExit.Set();
			_notificationThread = null;
			_delayedNotificationThread = null;
			_delayedUnblockingThread = null;
			lock(_pendingNotifications)
			{
				_pendingNotifications.Clear();
			}
			lock(_delayedNotifications)
			{
				_delayedNotifications.Clear();
			}
			lock(_delayedUnblocks)
			{
				_delayedUnblocks.Clear();
			}
			#if TRACE_FS_EVENTS
			_sw.Dispose();
			#endif
			_evGotNotification.Close();
			_evGotDelayedNotification.Close();
			_evGotDelayedUnblock.Close();
		}

		#endregion
	}
}
