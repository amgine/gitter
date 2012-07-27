namespace gitter.Git
{
	/// <summary>Hook names.</summary>
	public static class Hooks
	{
		/// <summary>applypatch-msg</summary>
		public const string ApplyPatchMsg = "applypatch-msg";

		/// <summary>pre-applypatch</summary>
		public const string PreApplyPatch = "pre-applypatch";

		/// <summary>post-applypatch</summary>
		public const string PostApplyPatch = "post-applypatch";

		/// <summary>pre-commit</summary>
		public const string PreCommit = "pre-commit";

		/// <summary>prepare-commit-msg</summary>
		public const string PrepareCommitMsg = "prepare-commit-msg";

		/// <summary>commit-msg</summary>
		public const string CommitMsg = "commit-msg";

		/// <summary>post-commit</summary>
		public const string PostCommit = "post-commit";

		/// <summary>pre-rebase</summary>
		public const string PreRebase = "pre-rebase";

		/// <summary>post-checkout</summary>
		public const string PostCheckout = "post-checkout";

		/// <summary>post-merge</summary>
		public const string PostMerge = "post-merge";

		/// <summary>pre-receive</summary>
		public const string PreReceive = "pre-receive";

		/// <summary>update</summary>
		public const string Update = "update";

		/// <summary>post-receive</summary>
		public const string PostReceive = "post-receive";

		/// <summary>post-update</summary>
		public const string PostUpdate = "post-update";

		/// <summary>pre-auto-gc</summary>
		public const string PreAutoGC = "pre-auto-gc";

		/// <summary>post-rewrite</summary>
		public const string PostRewrite = "post-rewrite";
	}
}
