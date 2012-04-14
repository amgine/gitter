namespace gitter.Git
{
	public static class RepositoryNotifications
	{
		public static readonly object CheckoutNotification			= new object();
		public static readonly object SubmodulesChangedNotification	= new object();
		public static readonly object RepositoryRemovedNotification	= new object();
		public static readonly object ConfigUpdatedNotification		= new object();
		public static readonly object IndexUpdatedNotification		= new object();
		public static readonly object WorktreeUpdatedNotification	= new object();
		public static readonly object BranchChangedNotification		= new object();
		public static readonly object TagChangedNotification		= new object();
		public static readonly object StashChangedNotification		= new object();
		public static readonly object RemoteRemovedNotification		= new object();
		public static readonly object RemoteCreatedNotification		= new object();
	}
}
