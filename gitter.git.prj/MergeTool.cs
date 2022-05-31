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

namespace gitter.Git;

using System;
using System.Collections.Generic;

using gitter.Framework;

/// <summary>Describes an external merge tool used by 'git mergetool' command.</summary>
public sealed class MergeTool : INamedObject
{
	#region Static

	public static readonly MergeTool kdiff3;
	public static readonly MergeTool tkdiff;
	public static readonly MergeTool meld;
	public static readonly MergeTool xxdiff;
	public static readonly MergeTool emerge;
	public static readonly MergeTool vimdiff;
	public static readonly MergeTool gvimdiff;
	public static readonly MergeTool ecmerge;
	public static readonly MergeTool diffuse;
	public static readonly MergeTool tortoisemerge;
	public static readonly MergeTool opendiff;
	public static readonly MergeTool p4merge;
	public static readonly MergeTool araxis;

	private static readonly Dictionary<string, MergeTool> _tools;

	public static MergeTool GetByName(string name) => _tools[name];

	public static MergeTool GetCreateByName(string name)
	{
		if(!_tools.TryGetValue(name, out var res))
		{
			res = new MergeTool(name);
		}
		return res;
	}

	public static IEnumerable<MergeTool> KnownTools
		=> _tools.Values;

	public static int KnownToolsCount => _tools.Count;

	#endregion

	#region .ctor

	static MergeTool()
	{
		_tools = new Dictionary<string, MergeTool>(capacity: 13)
		{
			{ "kdiff3",			kdiff3			= new MergeTool("kdiff3",			@"http://sourceforge.net/projects/kdiff3/files/", true, true) },
			{ "tkdiff",			tkdiff			= new MergeTool("tkdiff",			@"http://sourceforge.net/projects/tkdiff/files/", true, true) },
			{ "meld",			meld			= new MergeTool("meld",				@"http://ftp.gnome.org/pub/gnome/sources/meld/", false, true) },
			{ "xxdiff",			xxdiff			= new MergeTool("xxdiff",			@"http://sourceforge.net/projects/xxdiff/files/", false, true) },
			{ "emerge",			emerge			= new MergeTool("emerge",			@"", false, true) },
			{ "vimdiff",		vimdiff			= new MergeTool("vimdiff",			@"", false, true) },
			{ "gvimdiff",		gvimdiff		= new MergeTool("gvimdiff",			@"", false, true) },
			{ "ecmerge",		ecmerge			= new MergeTool("ecmerge",			@"http://www.elliecomputing.com/Download/download_form.asp", true, true) },
			{ "diffuse",		diffuse			= new MergeTool("diffuse",			@"http://diffuse.sourceforge.net/download.html", true, true) },
			{ "tortoisemerge",	tortoisemerge	= new MergeTool("tortoisemerge",	@"http://tortoisesvn.net/downloads", true, false) },
			{ "opendiff",		opendiff		= new MergeTool("opendiff",			@"", false, true) },
			{ "p4merge",		p4merge			= new MergeTool("p4merge",			@"http://www.perforce.com/perforce/downloads/index.html", true, true) },
			{ "araxis",			araxis			= new MergeTool("araxis",			@"http://www.araxis.com/merge/index.html", true, false) },
		};
	}

	internal MergeTool(string name)
	{
		Name = name;
	}

	private MergeTool(string name, string url, bool supportsWin, bool supportsLinux)
	{
		Name = name;
		DownloadUrl = url;
		SupportsWin = supportsWin;
		SupportsLinux = supportsLinux;
	}

	#endregion

	#region Properties

	public string Name { get; }

	public string DownloadUrl { get; }

	public bool SupportsWin { get; }

	public bool SupportsLinux { get; }

	#endregion

	/// <summary>Returns a <see cref="System.String"/> that represents this instance.</summary>
	/// <returns>A <see cref="System.String"/> that represents this instance.</returns>
	public override string ToString() => Name;
}
