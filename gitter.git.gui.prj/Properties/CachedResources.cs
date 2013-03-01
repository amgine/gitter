namespace gitter.Git.Gui
{
	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class CachedResources
	{
		private static readonly CachedBitmapResources _bitmaps = new CachedBitmapResources(Resources.ResourceManager);
		private static readonly CachedIconResources _icons = new CachedIconResources(Resources.ResourceManager);

		public static CachedBitmapResources Bitmaps
		{
			get { return _bitmaps; }
		}

		public static CachedIconResources Icons
		{
			get { return _icons; }
		}
	}
}
