namespace gitter.Git
{
	using System;

	/// <summary>Represents a branch on remote repository (not a remote tracking branch).</summary>
	public sealed class RemoteRepositoryBranch: BaseRemoteReference
	{
		internal RemoteRepositoryBranch(RemoteReferencesCollection refs, string name, string hash)
			: base(refs, name, hash)
		{
		}

		protected override void DeleteCore()
		{
			References.RemoveBranch(this);
		}

		public override ReferenceType ReferenceType
		{
			get { return ReferenceType.LocalBranch; }
		}
	}
}
