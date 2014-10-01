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

//#define TRACE_FS_EVENTS

namespace gitter.Git
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Threading;

	using gitter.Framework.Services;

	/// <summary>Watches git repository and notifies about external changes.</summary>
	public sealed class RepositoryMonitor : IRepositoryMonitor, IDisposable
	{
		private const int _notificationDelayTime = 750;
		private const int _unblockDelayTime = 750;

		#if TRACE_FS_EVENTS
		private static readonly LoggingService Log = new LoggingService("Monitor");
		#endif

		#region Data

		private readonly Repository _repository;
		private FileSystemWatcher _fswWorkDir;
		private FileSystemWatcher _fswGitDir;

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
		private bool _isEnabled;
		private bool _isDisposed;

		#endregion

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
			private readonly RepositoryMonitor _monitor;
			private readonly object[] _notifications;
			private readonly object _key;

			public NotificationsBlockToken(RepositoryMonitor monitor, object key, object[] notifications)
			{
				Assert.IsNotNull(monitor);

				_monitor = monitor;
				_notifications = notifications;
				_key = key;
			}

			public void Dispose()
			{
				if(_notifications != null && _notifications.Length != 0)
				{
					_monitor.UnblockNotifications(_key, _notifications);
				}
			}
		}

		private sealed class EmptyBlockToken : IDisposable
		{
			public EmptyBlockToken()
			{
			}

			public void Dispose()
			{
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
			Verify.Argument.IsNotNull(repository, "repository");

			_repository = repository;

			_blockedNotifications = new List<NotificationBlock>();
			_pendingNotifications = new Queue<IRepositoryChangedNotification>();
			_delayedNotifications = new Queue<IRepositoryChangedNotification>();
			_delayedUnblocks      = new Queue<NotificationDelayedUnblock>();
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
			Log.Debug(string.Format("{0} {1}", e.ChangeType, e.Name));
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
						Log.Debug("Detected possible stash change");
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
						Log.Debug(string.Format("Detected possible branch change: {0}", name));
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
						Log.Debug(string.Format("Detected possible remote branch change: {0}", name));
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
							Log.Debug(string.Format("Detected possible tag removal: {0}", name));
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
							Log.Debug("Repository destruction detected");
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
									Log.Debug("Checkout detected");
									#endif
									if(!IsBlocked(RepositoryNotifications.Checkout))
									{
										EmitNotification(new CheckoutNotification());
									}
									break;
								case "config" + GitConstants.LockPostfix:
									#if TRACE_FS_EVENTS
									Log.Debug("Config update detected");
									#endif
									if(!IsBlocked(RepositoryNotifications.ConfigUpdated))
									{
										EmitNotification(new ConfigUpdatedNotification());
									}
									break;
								case "index" + GitConstants.LockPostfix:
									#if TRACE_FS_EVENTS
									Log.Debug("Index update detected");
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
			Log.Debug(string.Format("{0} {1}", e.ChangeType, e.Name));
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
			if(e == null || e.OldName == null)
			{
				return;
			}
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
							Log.Debug(string.Format("Detected branch creation: {0}", newName));
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
							Log.Debug(string.Format("Detected remote branch creation: {0}", newName));
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
							Log.Debug(string.Format("Detected tag creation: {0}", newName));
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
			if(e == null || e.OldName == null)
			{
				return;
			}
			#if TRACE_FS_EVENTS
			Log.Debug(string.Format("{0} {1} -> {2}", e.ChangeType, e.OldName, e.Name));
			#endif
			switch(GetChangedPath(e.OldName))
			{
				case ChangedPath.WorkDir:
					#if TRACE_FS_EVENTS
					Log.Debug(string.Format("Detected working directory file rename: {0} -> {1]", e.OldName , e.Name));
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
			get { return _isEnabled; }
			set
			{
				Verify.State.IsFalse(IsDisposed, "RepositoryMonitor is disposed.");

				if(_isEnabled != value)
				{
					if(_isEnabled)
					{
						Stop();
					}
					_isEnabled = value;
					if(_isEnabled)
					{
						Start();
					}
				}
			}
		}

		private void Stop()
		{
			_fswGitDir.EnableRaisingEvents = false;
			_fswWorkDir.EnableRaisingEvents = false;

			_evExit.Set();

			_fswGitDir.Deleted -= OnGitDirDeleted;
			_fswGitDir.Renamed -= OnGitDirRenamed;

			_fswWorkDir.Created -= OnWorkDirCreated;
			_fswWorkDir.Deleted -= OnWorkDirDeleted;
			_fswWorkDir.Renamed -= OnWorkDirRenamed;
			_fswWorkDir.Changed -= OnWorkDirChanged;

			_fswGitDir.Dispose();
			_fswWorkDir.Dispose();

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

			_notificationThread.Join();
			_delayedNotificationThread.Join();
			_delayedUnblockingThread.Join();

			_evGotNotification.Dispose();
			_evGotDelayedNotification.Dispose();
			_evGotDelayedUnblock.Dispose();
			_evExit.Dispose();

			_fswGitDir = null;
			_fswWorkDir = null;

			_evGotNotification = null;
			_evGotDelayedNotification = null;
			_evGotDelayedUnblock = null;
			_evExit = null;
		}

		private void Start()
		{
			_fswGitDir = new FileSystemWatcher(_repository.GitDirectory)
			{
				NotifyFilter =
					NotifyFilters.CreationTime |
					NotifyFilters.DirectoryName |
					NotifyFilters.FileName |
					NotifyFilters.LastWrite,
				IncludeSubdirectories = true,
			};
			_fswWorkDir = new FileSystemWatcher(_repository.WorkingDirectory)
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
			if(IsDisposed || !IsEnabled)
			{
				return new EmptyBlockToken();
			}
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
				if(hadAny && _evGotDelayedUnblock != null)
				{
					_evGotDelayedUnblock.Set();
				}
			}
		}

		#region IDisposable

		public bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		public void Dispose()
		{
			if(!IsDisposed)
			{
				if(IsEnabled)
				{
					Stop();
				}
				IsDisposed = true;
			}
		}

		#endregion
	}
}
