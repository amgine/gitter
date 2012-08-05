namespace gitter.Git.Gui
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class FileStatusIcons
	{
		public static readonly Bitmap ImgUnmerged			= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayConflict");
		public static readonly Bitmap ImgStagedAdded		= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayAddStaged");
		public static readonly Bitmap ImgStagedRemoved		= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayDelStaged");
		public static readonly Bitmap ImgStagedModified		= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayEditStaged");
		public static readonly Bitmap ImgUnstagedUntracked	= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayAdd");
		public static readonly Bitmap ImgUnstagedModified	= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayEdit");
		public static readonly Bitmap ImgUnstagedRemoved	= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayDel");
		public static readonly Bitmap ImgModeChanged		= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayChmod");
		public static readonly Bitmap ImgRenamed			= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayRename");
		public static readonly Bitmap ImgCopied				= CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayCopy");
	}
}
