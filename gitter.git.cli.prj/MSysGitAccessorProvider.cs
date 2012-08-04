namespace gitter.Git.AccessLayer.CLI
{
	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	/// <summary>Provides accessor which works through MSysGit command line interface.</summary>
	public sealed class MSysGitAccessorProvider : IGitAccessorProvider
	{
		/// <summary>Returns string used to identify git accessor.</summary>
		public string Name
		{
			get { return "MSysGit"; }
		}

		/// <summary>Returns string to represent accessor in GUI.</summary>
		public string DisplayName
		{
			get { return Resources.StrProviderName; }
		}

		/// <summary>Creates git accessor.</summary>
		/// <returns>Created git accessor.</returns>
		public IGitAccessor CreateAccessor()
		{
			return new GitCLI(this);
		}
	}
}
