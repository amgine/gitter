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
			_hooks = new Dictionary<string, Hook>()
			{
				{"applypatch-msg",		_applyPatchMsg		= new Hook(repository, "applypatch-msg")},
				{"pre-applypatch",		_preApplyPatch		= new Hook(repository, "pre-applypatch")},
				{"post-applypatch",		_postApplyPatch		= new Hook(repository, "post-applypatch")},
				{"pre-commit",			_preCommit			= new Hook(repository, "pre-commit")},
				{"prepare-commit-msg",	_prepareCommitMsg	= new Hook(repository, "prepare-commit-msg")},
				{"commit-msg",			_commitMsg			= new Hook(repository, "commit-msg")},
				{"post-commit",			_postCommit			= new Hook(repository, "post-commit")},
				{"pre-rebase",			_preRebase			= new Hook(repository, "pre-rebase")},
				{"post-checkout",		_postCheckout		= new Hook(repository, "post-checkout")},
				{"post-merge",			_postMerge			= new Hook(repository, "post-merge")},
				{"pre-receive",			_preReceive			= new Hook(repository, "pre-receive")},
				{"update",				_update				= new Hook(repository, "update")},
				{"post-receive",		_postReceive		= new Hook(repository, "post-receive")},
				{"post-update",			_postUpdate			= new Hook(repository, "post-update")},
				{"pre-auto-gc",			_preAutoGC			= new Hook(repository, "pre-auto-gc")},
				{"post-rewrite",		_postRewrite		= new Hook(repository, "post-rewrite")},
			};
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
