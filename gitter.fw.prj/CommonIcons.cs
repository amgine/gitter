#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2022  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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

namespace gitter.Framework;

public static class CommonIcons
{
	static IImageProvider CreateProvider(string name)
		=> new ScaledImageProvider(CachedResources.ScaledBitmaps, name);

	public static class CheckBox
	{
		static IImageProvider CreateProvider(string name)
			=> new ScaledImageProvider(CachedResources.ScaledBitmaps, "checkbox." + name);

		public static readonly IImageProvider Checked            = CreateProvider(@"checked");
		public static readonly IImageProvider CheckedHover       = CreateProvider(@"checked.hover");
		public static readonly IImageProvider Unchecked          = CreateProvider(@"unchecked");
		public static readonly IImageProvider UncheckedHover     = CreateProvider(@"unchecked.hover");
		public static readonly IImageProvider Indeterminate      = CreateProvider(@"indeterminate");
		public static readonly IImageProvider IndeterminateHover = CreateProvider(@"indeterminate.hover");
	}

	public static class TreeLines
	{
		static IImageProvider CreateProvider(string name)
			=> new ScaledImageProvider(CachedResources.ScaledBitmaps, "treelines." + name);

		public static readonly IImageProvider Plus       = CreateProvider(@"plus");
		public static readonly IImageProvider PlusHover  = CreateProvider(@"plus.hover");
		public static readonly IImageProvider Minus      = CreateProvider(@"minus");
		public static readonly IImageProvider MinusHover = CreateProvider(@"minus.hover");
	}

	public static class Log
	{
		static IImageProvider CreateProvider(string name)
			=> new ScaledImageProvider(CachedResources.ScaledBitmaps, "log." + name);

		public static readonly IImageProvider Debug       = CreateProvider(@"debug");
		public static readonly IImageProvider Information = CreateProvider(@"info");
		public static readonly IImageProvider Warning     = CreateProvider(@"warning");
		public static readonly IImageProvider Error       = CreateProvider(@"error");
	}

	public static readonly IImageProvider Action        = CreateProvider(@"action");
	public static readonly IImageProvider ActionHover   = CreateProvider(@"action.hover");
	public static readonly IImageProvider Terminal      = CreateProvider(@"terminal");
	public static readonly IImageProvider VSCode        = CreateProvider(@"vscode");
	public static readonly IImageProvider Solution      = CreateProvider(@"solution");
	public static readonly IImageProvider User          = CreateProvider(@"user");
	public static readonly IImageProvider Refresh       = CreateProvider(@"refresh");
	public static readonly IImageProvider Init          = CreateProvider(@"init");
	public static readonly IImageProvider Clone         = CreateProvider(@"clone");
	public static readonly IImageProvider Test          = CreateProvider(@"test");
	public static readonly IImageProvider TestEmpty     = CreateProvider(@"test.empty");
	public static readonly IImageProvider Delete        = CreateProvider(@"delete");
	public static readonly IImageProvider Gravatar      = CreateProvider(@"gravatar");
	public static readonly IImageProvider Folder        = CreateProvider(@"folder");
	public static readonly IImageProvider Branch        = CreateProvider(@"branch");
	public static readonly IImageProvider Tag           = CreateProvider(@"tag");
	public static readonly IImageProvider ClipboardCopy = CreateProvider(@"clipboard.copy");
	public static readonly IImageProvider File          = CreateProvider(@"file.default");
	public static readonly IImageProvider SearchNext    = CreateProvider(@"search.next");
	public static readonly IImageProvider SearchPrev    = CreateProvider(@"search.prev");
	public static readonly IImageProvider SearchClose   = CreateProvider(@"search.close");
	public static readonly IImageProvider NavBack       = CreateProvider(@"nav.back");
	public static readonly IImageProvider NavForward    = CreateProvider(@"nav.forward");
	public static readonly IImageProvider WebBrowser    = CreateProvider(@"web.browser");
}
