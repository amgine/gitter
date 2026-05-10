#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2025  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
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
class IconPackManifestParserTests
{
	private const string SampleJson = """
		{
		  "iconDefinitions": {
		    "_file":            { "iconPath": "./icons/file.svg" },
		    "_folder":          { "iconPath": "./icons/folder.svg" },
		    "_folder_open":     { "iconPath": "./icons/folder-open.svg" },
		    "_html":            { "iconPath": "./icons/html.svg", "fontColor": "#e44d26" },
		    "_typescript":      "./icons/ts.svg",
		    "_node_modules":    { "iconPath": "./icons/node_modules.svg" }
		  },
		  "file": "_file",
		  "folder": "_folder",
		  "folderExpanded": "_folder_open",
		  "fileExtensions": {
		    "html": "_html",
		    "htm":  "_html",
		    "ts":   "_typescript"
		  },
		  "fileNames": {
		    "package.json": "_typescript"
		  },
		  "folderNames": {
		    "node_modules": "_node_modules"
		  },
		  "languageIds": {
		    "typescript": "_typescript"
		  }
		}
		""";

	[Test]
	public void Parse_SampleManifest_PopulatesAllSections()
	{
		var manifest = IconPackManifestParser.Parse(SampleJson);

		Assert.That(manifest, Is.Not.Null);
		Assert.That(manifest.File,           Is.EqualTo("_file"));
		Assert.That(manifest.Folder,         Is.EqualTo("_folder"));
		Assert.That(manifest.FolderExpanded, Is.EqualTo("_folder_open"));

		Assert.That(manifest.IconDefinitions,     Has.Count.EqualTo(6));
		Assert.That(manifest.FileExtensions,      Has.Count.EqualTo(3));
		Assert.That(manifest.FileNames,           Has.Count.EqualTo(1));
		Assert.That(manifest.FolderNames,         Has.Count.EqualTo(1));
		Assert.That(manifest.LanguageIds,         Has.Count.EqualTo(1));
	}

	[Test]
	public void Parse_IconDefinitionWithObject_PopulatesIconPathAndFontColor()
	{
		var manifest = IconPackManifestParser.Parse(SampleJson);

		var html = manifest.IconDefinitions["_html"];
		Assert.That(html.IconPath,  Is.EqualTo("./icons/html.svg"));
		Assert.That(html.FontColor, Is.EqualTo("#e44d26"));
	}

	[Test]
	public void Parse_IconDefinitionAsString_TreatsValueAsIconPath()
	{
		var manifest = IconPackManifestParser.Parse(SampleJson);

		var ts = manifest.IconDefinitions["_typescript"];
		Assert.That(ts.IconPath, Is.EqualTo("./icons/ts.svg"));
	}

	[Test]
	public void Parse_FileExtensions_MapsExtensionToIconDefName()
	{
		var manifest = IconPackManifestParser.Parse(SampleJson);

		Assert.That(manifest.FileExtensions["html"], Is.EqualTo("_html"));
		Assert.That(manifest.FileExtensions["htm"],  Is.EqualTo("_html"));
		Assert.That(manifest.FileExtensions["ts"],   Is.EqualTo("_typescript"));
	}

	[Test]
	public void Parse_FolderNames_MapsNameToIconDefName()
	{
		var manifest = IconPackManifestParser.Parse(SampleJson);
		Assert.That(manifest.FolderNames["node_modules"], Is.EqualTo("_node_modules"));
	}

	[Test]
	public void Parse_TrailingCommasAndComments_AreTolerated()
	{
		var json = """
			{
			  // top-level
			  "iconDefinitions": {
			    "_file": { "iconPath": "./f.svg", },
			  },
			  "file": "_file",
			}
			""";
		var manifest = IconPackManifestParser.Parse(json);
		Assert.That(manifest.File, Is.EqualTo("_file"));
		Assert.That(manifest.IconDefinitions["_file"].IconPath, Is.EqualTo("./f.svg"));
	}

	[Test]
	public void Parse_MissingOptionalSections_ProducesEmptyDictionaries()
	{
		var manifest = IconPackManifestParser.Parse("""{ "file": "_file" }""");

		Assert.That(manifest.File, Is.EqualTo("_file"));
		Assert.That(manifest.IconDefinitions, Is.Empty);
		Assert.That(manifest.FileExtensions,  Is.Empty);
		Assert.That(manifest.FileNames,       Is.Empty);
		Assert.That(manifest.FolderNames,     Is.Empty);
		Assert.That(manifest.LanguageIds,     Is.Empty);
	}

	[Test]
	public void Parse_UnknownIconDefinitionFields_AreIgnored()
	{
		var json = """
			{
			  "iconDefinitions": {
			    "_x": { "iconPath": "./x.svg", "extraField": 42, "nested": { "a": 1 } }
			  }
			}
			""";
		var manifest = IconPackManifestParser.Parse(json);
		Assert.That(manifest.IconDefinitions["_x"].IconPath, Is.EqualTo("./x.svg"));
	}
}
