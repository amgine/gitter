namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	/// <summary>Repository <see cref="Hook"/> objects collection.</summary>
	public sealed class HooksCollection : GitObject, IEnumerable<Hook>
	{
		#region Data

		private readonly Dictionary<string, Hook> _hooks;
		private readonly Hook _applyPatchMsg;
		private readonly Hook _preApplyPatch;
		private readonly Hook _postApplyPatch;
		private readonly Hook _preCommit;
		private readonly Hook _prepareCommitMsg;
		private readonly Hook _commitMsg;
		private readonly Hook _postCommit;
		private readonly Hook _preRebase;
		private readonly Hook _postCheckout;
		private readonly Hook _postMerge;
		private readonly Hook _preReceive;
		private readonly Hook _update;
		private readonly Hook _postReceive;
		private readonly Hook _postUpdate;
		private readonly Hook _preAutoGC;
		private readonly Hook _postRewrite;

		#endregion

		/// <summary>Create <see cref="HooksCollection"/>.</summary>
		/// <param name="repository">Related <see cref="Repository"/>.</param>
		internal HooksCollection(Repository repository)
			: base(repository)
		{
			_hooks = new Dictionary<string, Hook>();
			AddHook(_applyPatchMsg		= new Hook(repository, Hooks.ApplyPatchMsg));
			AddHook(_preApplyPatch		= new Hook(repository, Hooks.PreApplyPatch));
			AddHook(_postApplyPatch		= new Hook(repository, Hooks.PostApplyPatch));
			AddHook(_preCommit			= new Hook(repository, Hooks.PreCommit));
			AddHook(_prepareCommitMsg	= new Hook(repository, Hooks.PrepareCommitMsg));
			AddHook(_commitMsg			= new Hook(repository, Hooks.CommitMsg));
			AddHook(_postCommit			= new Hook(repository, Hooks.PostCommit));
			AddHook(_preRebase			= new Hook(repository, Hooks.PreRebase));
			AddHook(_postCheckout		= new Hook(repository, Hooks.PostCheckout));
			AddHook(_postMerge			= new Hook(repository, Hooks.PostMerge));
			AddHook(_preReceive			= new Hook(repository, Hooks.PreReceive));
			AddHook(_update				= new Hook(repository, Hooks.Update));
			AddHook(_postReceive		= new Hook(repository, Hooks.PostReceive));
			AddHook(_postUpdate			= new Hook(repository, Hooks.PostUpdate));
			AddHook(_preAutoGC			= new Hook(repository, Hooks.PreAutoGC));
			AddHook(_postRewrite		= new Hook(repository, Hooks.PostRewrite));
		}

		private void AddHook(Hook hook)
		{
			_hooks.Add(hook.Name, hook);
		}

		public Hook ApplyPatchMsg
		{
			get { return _applyPatchMsg; }
		}

		public Hook PreApplyPatch
		{
			get { return _preApplyPatch; }
		}

		public Hook PostApplyPatch
		{
			get { return _postApplyPatch; }
		}

		public Hook PreCommit
		{
			get { return _preCommit; }
		}

		public Hook PrepareCommitMsg
		{
			get { return _prepareCommitMsg; }
		}

		public Hook CommitMsg
		{
			get { return _commitMsg; }
		}

		public Hook PostCommit
		{
			get { return _postCommit; }
		}

		public Hook PreRebase
		{
			get { return _preRebase; }
		}

		public Hook PostCheckout
		{
			get { return _postCheckout; }
		}

		public Hook PostMerge
		{
			get { return _postMerge; }
		}

		public Hook PreReceive
		{
			get { return _preReceive; }
		}

		public Hook Update
		{
			get { return _update; }
		}

		public Hook PostReceive
		{
			get { return _postReceive; }
		}

		public Hook PostUpdate
		{
			get { return _postUpdate; }
		}

		public Hook PreAutoGC
		{
			get { return _preAutoGC; }
		}

		public Hook PostRewrite
		{
			get { return _postRewrite; }
		}

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
