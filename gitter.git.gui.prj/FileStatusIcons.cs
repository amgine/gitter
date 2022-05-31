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

namespace gitter.Git.Gui;

using gitter.Framework;

static class FileStatusIcons
{
	public static readonly IImageProvider ImgUnmerged          = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Conflict);
	public static readonly IImageProvider ImgStagedAdded       = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.AddStaged);
	public static readonly IImageProvider ImgStagedRemoved     = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.DeleteStaged);
	public static readonly IImageProvider ImgStagedModified    = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.EditStaged);
	public static readonly IImageProvider ImgUnstagedUntracked = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Add);
	public static readonly IImageProvider ImgUnstagedModified  = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Edit);
	public static readonly IImageProvider ImgUnstagedRemoved   = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Delete);
	public static readonly IImageProvider ImgModeChanged       = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Chmod);
	public static readonly IImageProvider ImgRenamed           = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Rename);
	public static readonly IImageProvider ImgCopied            = new CombinedImageProvider(CommonIcons.File, Icons.Overlays.Copy);
}
