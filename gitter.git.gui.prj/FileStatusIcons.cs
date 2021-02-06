#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.Gui
{
	using System;
	using System.Drawing;

	using Resources = gitter.Git.Gui.Properties.Resources;

	static class FileStatusIcons
	{
		public static readonly Bitmap ImgUnmerged          = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayConflict");
		public static readonly Bitmap ImgStagedAdded       = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayAddStaged");
		public static readonly Bitmap ImgStagedRemoved     = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayDelStaged");
		public static readonly Bitmap ImgStagedModified    = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayEditStaged");
		public static readonly Bitmap ImgUnstagedUntracked = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayAdd");
		public static readonly Bitmap ImgUnstagedModified  = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayEdit");
		public static readonly Bitmap ImgUnstagedRemoved   = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayDel");
		public static readonly Bitmap ImgModeChanged       = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayChmod");
		public static readonly Bitmap ImgRenamed           = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayRename");
		public static readonly Bitmap ImgCopied            = CachedResources.Bitmaps.CombineBitmaps("ImgDocument", "ImgOverlayCopy");
	}
}
