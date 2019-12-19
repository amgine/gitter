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

	/// <summary>Repository <see cref="Hook"/> objects collection.</summary>
	public sealed class HooksCollection : GitObject, IEnumerable<Hook>
	{
		#region Data

		private readonly Dictionary<string, Hook> _hooks;

		#endregion

		/// <summary>Create <see cref="HooksCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal HooksCollection(Repository repository)
			: base(repository)
		{
			_hooks                      = new Dictionary<string, Hook>();
			AddHook(ApplyPatchMsg		= new Hook(repository, Hooks.ApplyPatchMsg));
			AddHook(PreApplyPatch		= new Hook(repository, Hooks.PreApplyPatch));
			AddHook(PostApplyPatch		= new Hook(repository, Hooks.PostApplyPatch));
			AddHook(PreCommit			= new Hook(repository, Hooks.PreCommit));
			AddHook(PrepareCommitMsg	= new Hook(repository, Hooks.PrepareCommitMsg));
			AddHook(CommitMsg			= new Hook(repository, Hooks.CommitMsg));
			AddHook(PostCommit			= new Hook(repository, Hooks.PostCommit));
			AddHook(PreRebase			= new Hook(repository, Hooks.PreRebase));
			AddHook(PostCheckout		= new Hook(repository, Hooks.PostCheckout));
			AddHook(PostMerge			= new Hook(repository, Hooks.PostMerge));
			AddHook(PreReceive			= new Hook(repository, Hooks.PreReceive));
			AddHook(Update				= new Hook(repository, Hooks.Update));
			AddHook(PostReceive		    = new Hook(repository, Hooks.PostReceive));
			AddHook(PostUpdate			= new Hook(repository, Hooks.PostUpdate));
			AddHook(PreAutoGC			= new Hook(repository, Hooks.PreAutoGC));
			AddHook(PostRewrite		    = new Hook(repository, Hooks.PostRewrite));
		}

		private void AddHook(Hook hook)
		{
			_hooks.Add(hook.Name, hook);
		}

		public Hook ApplyPatchMsg { get; }

		public Hook PreApplyPatch { get; }

		public Hook PostApplyPatch { get; }

		public Hook PreCommit { get; }

		public Hook PrepareCommitMsg { get; }

		public Hook CommitMsg { get; }

		public Hook PostCommit { get; }

		public Hook PreRebase { get; }

		public Hook PostCheckout { get; }

		public Hook PostMerge { get; }

		public Hook PreReceive { get; }

		public Hook Update { get; }

		public Hook PostReceive { get; }

		public Hook PostUpdate { get; }

		public Hook PreAutoGC { get; }

		public Hook PostRewrite { get; }

		public int Count
		{
			get { return _hooks.Count; }
		}

		public Hook this[string name]
		{
			get { return _hooks[name]; }
		}

		public bool TryGetHook(string name, out Hook hook)
		{
			return _hooks.TryGetValue(name, out hook);
		}

		public Hook TryGetHook(string name)
		{
			Hook hook;
			if(_hooks.TryGetValue(name, out hook))
			{
				return hook;
			}
			return null;
		}

		#region IEnumerable<Hook> Members

		public IEnumerator<Hook> GetEnumerator()
		{
			return _hooks.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _hooks.Values.GetEnumerator();
		}

		#endregion
	}
}
