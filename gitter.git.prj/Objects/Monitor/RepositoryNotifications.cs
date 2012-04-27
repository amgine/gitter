namespace gitter.Git
{
	public static class RepositoryNotifications
	{
		public static readonly object Checkout			= new object();
		public static readonly object SubmodulesChanged	= new object();
		public static readonly object RepositoryRemoved	= new object();
		public static readonly object ConfigUpdated		= new object();
		public static readonly object IndexUpdated		= new object();
		public static readonly object WorktreeUpdated	= new object();
		public static readonly object BranchChanged		= new object();
		public static readonly object TagChanged		= new object();
		public static readonly object StashChanged		= new object();
		public static readonly object RemoteRemoved		= new object();
		public static readonly object RemoteCreated		= new object();
	}
}
