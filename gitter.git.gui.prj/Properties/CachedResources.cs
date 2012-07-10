namespace gitter.Git.Gui
{
	using System;

	using gitter.Framework;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class CachedResources
	{
		private static readonly CachedBitmapResources _bitmaps;
		private static readonly CachedIconResources _icons;

		static CachedResources()
		{
			_bitmaps = new CachedBitmapResources(Resources.ResourceManager);
			_icons = new CachedIconResources(Resources.ResourceManager);
		}

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
