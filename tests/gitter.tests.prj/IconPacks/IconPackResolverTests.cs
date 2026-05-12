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

using NUnit.Framework;

[TestFixture]
class IconPackResolverTests
{
	private static IconPackManifest BuildManifest()
	{
		var m = new IconPackManifest
		{
			File           = "_file",
			Folder         = "_folder",
			FolderExpanded = "_folder_open",
			IconDefinitions =
			{
				["_file"]            = new IconDefinition { IconPath = "./icons/file.svg" },
				["_folder"]          = new IconDefinition { IconPath = "./icons/folder.svg" },
				["_folder_open"]     = new IconDefinition { IconPath = "./icons/folder-open.svg" },
				["_typescript"]      = new IconDefinition { IconPath = "./icons/ts.svg" },
				["_typescript_def"]  = new IconDefinition { IconPath = "./icons/ts.d.ts.svg" },
				["_html"]            = new IconDefinition { IconPath = "./icons/html.svg" },
				["_node_modules"]    = new IconDefinition { IconPath = "./icons/node_modules.svg" },
				["_pkg_json"]        = new IconDefinition { IconPath = "./icons/pkg.svg" },
				["_node_modules_open"] = new IconDefinition { IconPath = "./icons/nm-open.svg" },
			},
			FileExtensions =
			{
				["ts"]    = "_typescript",
				["d.ts"]  = "_typescript_def",
				["html"]  = "_html",
			},
			FileNames =
			{
				["package.json"] = "_pkg_json",
			},
			LanguageIds =
			{
				["typescript"] = "_typescript",
			},
			FolderNames =
			{
				["node_modules"] = "_node_modules",
			},
			FolderNamesExpanded =
			{
				["node_modules"] = "_node_modules_open",
			},
		};
		return m;
	}

	[Test]
	public void ResolveFile_ExactName_PrefersFileNames()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("package.json"), Is.EqualTo("_pkg_json"));
	}

	[Test]
	public void ResolveFile_ExactNameTakesPriorityOverExtension()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("package.json"), Is.EqualTo("_pkg_json"));
	}

	[Test]
	public void ResolveFile_LongestExtensionWins()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("foo.d.ts"), Is.EqualTo("_typescript_def"));
		Assert.That(r.ResolveFile("foo.ts"),   Is.EqualTo("_typescript"));
	}

	[Test]
	public void ResolveFile_FallsBackToLanguageId()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("noext", "typescript"), Is.EqualTo("_typescript"));
	}

	[Test]
	public void ResolveFile_FallsBackToDefault()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("unknown.xyz"), Is.EqualTo("_file"));
	}

	[Test]
	public void ResolveFile_HandlesPathInput()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("/some/dir/package.json"), Is.EqualTo("_pkg_json"));
		Assert.That(r.ResolveFile(@"C:\dir\foo.ts"),         Is.EqualTo("_typescript"));
	}

	[Test]
	public void ResolveFile_IsCaseInsensitiveForFileNames()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("PACKAGE.JSON"), Is.EqualTo("_pkg_json"));
	}

	[Test]
	public void ResolveFile_IsCaseInsensitiveForExtensions()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFile("Foo.HTML"), Is.EqualTo("_html"));
	}

	[Test]
	public void ResolveFolder_ExactName_PrefersFolderNames()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFolder("node_modules"), Is.EqualTo("_node_modules"));
	}

	[Test]
	public void ResolveFolder_Expanded_UsesExpandedMap()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFolder("node_modules", expanded: true), Is.EqualTo("_node_modules_open"));
	}

	[Test]
	public void ResolveFolder_DefaultsApply()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveFolder("misc"),                 Is.EqualTo("_folder"));
		Assert.That(r.ResolveFolder("misc", expanded: true), Is.EqualTo("_folder_open"));
	}

	[Test]
	public void ResolveIconPath_LooksUpDefinition()
	{
		var r = new IconPackResolver(BuildManifest());
		Assert.That(r.ResolveIconPath("_file"), Is.EqualTo("./icons/file.svg"));
		Assert.That(r.ResolveIconPath("nope"),  Is.Null);
		Assert.That(r.ResolveIconPath(null),    Is.Null);
	}
}
