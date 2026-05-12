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

namespace gitter.IconPacks;

using System;
using System.IO;

using NUnit.Framework;

[TestFixture]
class IconPackLoaderTests
{
	[Test]
	public void LoadFromEmbeddedResources_BundledDefault_LoadsAndRendersFileIcon()
	{
		using var pack = IconPackLoader.LoadFromEmbeddedResources(
			typeof(IconPackLoader).Assembly,
			IconPackDiscovery.DefaultBundledResourceRoot,
			IconPackDiscovery.DefaultBundledPackName);

		Assert.That(pack.Manifest.File,   Is.EqualTo("_file"));
		Assert.That(pack.Manifest.Folder, Is.EqualTo("_folder"));

		using var bmp = pack.GetFileBitmap("foo.json", 16);
		Assert.That(bmp,         Is.Not.Null);
		Assert.That(bmp!.Width,  Is.EqualTo(16));
		Assert.That(bmp.Height,  Is.EqualTo(16));
	}

	[Test]
	public void LoadFromEmbeddedResources_BundledDefault_RendersFolderIcons()
	{
		using var pack = IconPackLoader.LoadFromEmbeddedResources(
			typeof(IconPackLoader).Assembly,
			IconPackDiscovery.DefaultBundledResourceRoot,
			IconPackDiscovery.DefaultBundledPackName);

		using var closed = pack.GetFolderBitmap("src", 16, expanded: false);
		using var open   = pack.GetFolderBitmap("src", 16, expanded: true);
		Assert.That(closed, Is.Not.Null);
		Assert.That(open,   Is.Not.Null);
	}

	[Test]
	public void LoadFromDirectory_RoundTripFromTempDir_Works()
	{
		var temp = Path.Combine(Path.GetTempPath(), $"gitter_pack_{Guid.NewGuid():N}");
		Directory.CreateDirectory(Path.Combine(temp, "icons"));
		try
		{
			File.WriteAllText(Path.Combine(temp, "icon-theme.json"),
				"""
				{
				  "iconDefinitions": { "_file": { "iconPath": "./icons/file.svg" } },
				  "file": "_file"
				}
				""");
			File.WriteAllText(Path.Combine(temp, "icons", "file.svg"),
				"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><rect width="24" height="24" fill="#888"/></svg>""");

			using var pack = IconPackLoader.LoadFromDirectory(temp);
			Assert.That(pack.Name, Is.EqualTo(new DirectoryInfo(temp).Name));
			using var bmp = pack.GetFileBitmap("anything.txt", 24);
			Assert.That(bmp,        Is.Not.Null);
			Assert.That(bmp!.Width, Is.EqualTo(24));
		}
		finally
		{
			try { Directory.Delete(temp, recursive: true); } catch { }
		}
	}

	[Test]
	public void Discover_NoUserDir_ReturnsBundledOnly()
	{
		var nowhere = Path.Combine(Path.GetTempPath(), $"gitter_no_{Guid.NewGuid():N}");
		var packs = IconPackDiscovery.Discover(nowhere, typeof(IconPackLoader).Assembly);
		Assert.That(packs, Has.Count.EqualTo(1));
		Assert.That(packs[0].Source, Is.EqualTo(IconPackSource.Bundled));
		Assert.That(packs[0].Name,   Is.EqualTo(IconPackDiscovery.DefaultBundledPackName));
	}

	[Test]
	public void Discover_FindsUserPacks()
	{
		var root = Path.Combine(Path.GetTempPath(), $"gitter_disc_{Guid.NewGuid():N}");
		var pack = Path.Combine(root, "myPack");
		Directory.CreateDirectory(Path.Combine(pack, "icons"));
		try
		{
			File.WriteAllText(Path.Combine(pack, "icon-theme.json"), """{ "file": "_file" }""");
			var found = IconPackDiscovery.Discover(root, typeof(IconPackLoader).Assembly);
			Assert.That(found, Has.Count.EqualTo(2));
			Assert.That(found[1].Source, Is.EqualTo(IconPackSource.User));
			Assert.That(found[1].Name,   Is.EqualTo("myPack"));
		}
		finally
		{
			try { Directory.Delete(root, recursive: true); } catch { }
		}
	}

	[Test]
	public void EnsureUserPacksDirectory_CreatesIfMissing()
	{
		var root = Path.Combine(Path.GetTempPath(), $"gitter_ensure_{Guid.NewGuid():N}");
		try
		{
			Assert.That(Directory.Exists(root), Is.False);
			var dir = IconPackDiscovery.EnsureUserPacksDirectory(root);
			Assert.That(dir, Is.EqualTo(root));
			Assert.That(Directory.Exists(root), Is.True);
		}
		finally
		{
			try { Directory.Delete(root, recursive: true); } catch { }
		}
	}
}
