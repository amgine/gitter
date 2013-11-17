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
	using System.Collections.Generic;

	using gitter.Git.AccessLayer;

	/// <summary>Reference change history log.</summary>
	public sealed class Reflog : GitObject, IEnumerable<ReflogRecord>
	{
		#region Static

		private static readonly Dictionary<Reflog, Delegate> _recordAddedHandlers =
			new Dictionary<Reflog, Delegate>();

		private static readonly Dictionary<Reflog, Delegate> _recordRemovedHandlers =
			new Dictionary<Reflog, Delegate>();

		#endregion

		#region Data

		private readonly Reference _reference;
		private readonly List<ReflogRecord> _reflog;

		#endregion

		#region Events

		public event EventHandler<ReflogRecordEventArgs> RecordAdded
		{
			add
			{
				lock(_recordAddedHandlers)
				{
					Delegate handler;
					if(_recordAddedHandlers.TryGetValue(this, out handler))
					{
						handler = Delegate.Combine(handler, value);
						_recordAddedHandlers[this] = handler;
					}
					else
					{
						_recordAddedHandlers.Add(this, value);
					}
				}
			}
			remove
			{
				lock(_recordAddedHandlers)
				{
					Delegate handler;
					if(_recordAddedHandlers.TryGetValue(this, out handler))
					{
						handler = Delegate.Remove(handler, value);
						if(handler == null || handler.GetInvocationList().Length == 0)
						{
							_recordAddedHandlers.Remove(this);
						}
						else
						{
							_recordAddedHandlers[this] = handler;
						}
					}
				}
			}
		}

		public event EventHandler<ReflogRecordEventArgs> RecordRemoved
		{
			add
			{
				lock(_recordRemovedHandlers)
				{
					Delegate handler;
					if(_recordRemovedHandlers.TryGetValue(this, out handler))
					{
						handler = Delegate.Combine(handler, value);
						_recordRemovedHandlers[this] = handler;
					}
					else
					{
						_recordRemovedHandlers.Add(this, value);
					}
				}
			}
			remove
			{
				lock(_recordRemovedHandlers)
				{
					Delegate handler;
					if(_recordRemovedHandlers.TryGetValue(this, out handler))
					{
						handler = Delegate.Remove(handler, value);
						if(handler == null || handler.GetInvocationList().Length == 0)
						{
							_recordRemovedHandlers.Remove(this);
						}
						else
						{
							_recordRemovedHandlers[this] = handler;
						}
					}
				}
			}
		}

		private void InvokeRecordAdded(ReflogRecord record)
		{
			EventHandler<ReflogRecordEventArgs> handler;
			lock(_recordAddedHandlers)
			{
				Delegate handlerDelegate;
				if(_recordAddedHandlers.TryGetValue(this, out handlerDelegate))
				{
					handler = (EventHandler<ReflogRecordEventArgs>)handlerDelegate;
				}
				else
				{
					handler = null;
				}
			}
			if(handler != null) handler(this, new ReflogRecordEventArgs(record));
		}

		private void InvokeRecordRemoved(ReflogRecord record)
		{
			EventHandler<ReflogRecordEventArgs> handler;
			lock(_recordRemovedHandlers)
			{
				Delegate handlerDelegate;
				if(_recordRemovedHandlers.TryGetValue(this, out handlerDelegate))
				{
					handler = (EventHandler<ReflogRecordEventArgs>)handlerDelegate;
				}
				else
				{
					handler = null;
				}
			}
			if(handler != null) handler(this, new ReflogRecordEventArgs(record));
		}

		#endregion

		#region .ctor

		internal Reflog(Reference reference)
			: base(reference.Repository)
		{
			_reference = reference;
			_reflog = new List<ReflogRecord>();
			Refresh();
		}

		#endregion

		#region Properties

		public Reference Reference
		{
			get { return _reference; }
		}

		public int Count
		{
			get { return _reflog.Count; }
		}

		public ReflogRecord this[int index]
		{
			get { return _reflog[index]; }
		}

		public object SyncRoot
		{
			get { return _reflog; }
		}

		#endregion

		public void Refresh()
		{
			var reflog = Repository.Accessor.QueryReflog.Invoke(
				new QueryReflogParameters(_reference.FullName));
			lock(SyncRoot)
			{
				if(reflog.Count < _reflog.Count)
				{
					for(int i = _reflog.Count - 1; i >= reflog.Count; --i)
					{
						_reflog[i].MarkAsDeleted();
						_reflog.RemoveAt(i);
					}
				}
				for(int i = 0; i < _reflog.Count; ++i)
				{
					ObjectFactories.UpdateReflogRecord(_reflog[i], reflog[i]);
				}
				for(int i = _reflog.Count; i < reflog.Count; ++i)
				{
					var reflogRecord = ObjectFactories.CreateReflogRecord(this, reflog[i]);
					_reflog.Add(reflogRecord);
					InvokeRecordAdded(reflogRecord);
				}
			}
		}

		internal void NotifyRecordAdded()
		{
			var data = Repository.Accessor.QueryReflog.Invoke(
				new QueryReflogParameters(_reference.FullName)
				{
					MaxCount = 1,
				});
			if(data.Count != 1) return;
			var record = data[0];
			lock(SyncRoot)
			{
				if(_reflog.Count != 0)
				{
					if(record.Revision.SHA1 == _reflog[0].Revision.Hash)
					{
						return;
					}
				}
				var item = ObjectFactories.CreateReflogRecord(this, record);
				_reflog.Insert(0, item);
				for(int i = 1; i < _reflog.Count; ++i)
				{
					_reflog[i].Index = i;
				}
				InvokeRecordAdded(item);
			}
		}

		#region IEnumerable<ReflogRecord>

		public IEnumerator<ReflogRecord> GetEnumerator()
		{
			return _reflog.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _reflog.GetEnumerator();
		}

		#endregion
	}
}
