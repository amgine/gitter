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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using gitter.Framework;

public sealed class RevisionReferencesCollection : IEnumerable<Reference>
{
	public struct Enumerator : IEnumerator<Reference>
	{
		private SortedDictionary<string, Reference>.ValueCollection.Enumerator _enumerator;
		private readonly bool _notEmpty;

		internal Enumerator(SortedDictionary<string, Reference>? container)
		{
			if(container is { Count: not 0 })
			{
				_enumerator = container.Values.GetEnumerator();
				_notEmpty   = true;
			}
		}

		public readonly Reference Current => _enumerator.Current;

		public bool MoveNext() => _notEmpty && _enumerator.MoveNext();

		readonly object IEnumerator.Current => _enumerator.Current;

		readonly void IEnumerator.Reset() { }

		public void Dispose()
		{
			if(_notEmpty) _enumerator.Dispose();
		}
	}

	public event EventHandler? Changed;

	private void OnChanged()
		=> Changed?.Invoke(this, EventArgs.Empty);

	private SortedDictionary<string, Reference>? _container;

	internal RevisionReferencesCollection()
	{
	}

	public LockType SyncRoot { get; } = new();

	public int Count
	{
		get { lock(SyncRoot) return _container is not null ? _container.Count : 0; }
	}

	public Reference this[string name]
	{
		get
		{
			lock(SyncRoot)
			{
				return _container is not null
					? _container[name]
					: throw new ArgumentException($"Reference '{name}' was not found.", nameof(name));
			}
		}
	}

	public bool Contains([NotNullWhen(returnValue: true)] string? referenceName)
	{
		if(referenceName is null) return false;

		lock(SyncRoot)
		{
			return _container is not null
				&& _container.ContainsKey(referenceName);
		}
	}

	public bool Contains([NotNullWhen(returnValue: true)] Reference? reference)
	{
		if(reference is null) return false;

		lock(SyncRoot)
		{
			return _container is not null
				&& _container.ContainsKey(reference.Name);
		}
	}

	private Many<T> GetRefs<T>()
		where T : Reference
	{
		lock(SyncRoot)
		{
			if(_container is not { Count: not 0 }) return Many<T>.None;
			var builder = new Many<T>.Builder();
			foreach(var reference in _container.Values)
			{
				if(reference is not T typedRef) continue;
				builder.Add(typedRef);
			}
			return builder;
		}
	}

	public Many<Branch> GetBranches() => GetRefs<Branch>();

	public Many<RemoteBranch> GetRemoteBranches() => GetRefs<RemoteBranch>();

	public Many<BranchBase> GetAllBranches() => GetRefs<BranchBase>();

	public Many<Tag> GetTags() => GetRefs<Tag>();

	internal void Remove(string reference)
	{
		Assert.IsNotNull(reference);

		lock(SyncRoot)
		{
			if(_container is null) return;
			if(!_container.Remove(reference)) return;
		}
		OnChanged();
	}

	internal void Remove(Reference reference)
	{
		Assert.IsNotNull(reference);

		lock(SyncRoot)
		{
			if(_container is null) return;
			if(!_container.Remove(reference.FullName)) return;
		}
		OnChanged();
	}

	internal void Rename(string oldName, Reference reference)
	{
		Assert.IsNeitherNullNorWhitespace(oldName);
		Verify.Argument.IsNotNull(reference);

		lock(SyncRoot)
		{
			if(_container is not null)
			{
				_container.Remove(oldName);
			}
			else
			{
				_container = [];
			}
			_container.Add(reference.FullName, reference);
		}
		OnChanged();
	}

	internal void Add(Reference reference)
	{
		Verify.Argument.IsNotNull(reference);

		lock(SyncRoot)
		{
			_container ??= [];
			_container.Add(reference.FullName, reference);
		}
		OnChanged();
	}

	public Enumerator GetEnumerator()
		=> new(_container);

	/// <inheritdoc/>
	IEnumerator<Reference> IEnumerable<Reference>.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
}
