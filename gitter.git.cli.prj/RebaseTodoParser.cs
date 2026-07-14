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

namespace gitter.Git.AccessLayer.CLI;

using System.Collections.Generic;
using System.IO;
using System.Text;

public static class RebaseTodoParser
{
	static readonly Dictionary<string, RebaseAction> ActionKeywords = new(System.StringComparer.OrdinalIgnoreCase)
	{
		["pick"]   = RebaseAction.Pick,
		["p"]      = RebaseAction.Pick,
		["reword"] = RebaseAction.Reword,
		["r"]      = RebaseAction.Reword,
		["edit"]   = RebaseAction.Edit,
		["e"]      = RebaseAction.Edit,
		["squash"] = RebaseAction.Squash,
		["s"]      = RebaseAction.Squash,
		["fixup"]  = RebaseAction.Fixup,
		["f"]      = RebaseAction.Fixup,
		["exec"]   = RebaseAction.Exec,
		["x"]      = RebaseAction.Exec,
		["drop"]   = RebaseAction.Drop,
		["d"]      = RebaseAction.Drop,
	};

	static readonly Dictionary<RebaseAction, string> ActionToKeyword = new()
	{
		[RebaseAction.Pick]   = "pick",
		[RebaseAction.Reword] = "reword",
		[RebaseAction.Edit]   = "edit",
		[RebaseAction.Squash] = "squash",
		[RebaseAction.Fixup]  = "fixup",
		[RebaseAction.Exec]   = "exec",
		[RebaseAction.Drop]   = "drop",
	};

	public static IReadOnlyList<RebaseTodoEntry> Read(string filePath)
	{
		var entries = new List<RebaseTodoEntry>();
		foreach(var line in File.ReadLines(filePath, Encoding.UTF8))
		{
			var trimmed = line.Trim();
			if(trimmed.Length == 0 || trimmed[0] == '#') continue;

			var firstSpace = trimmed.IndexOf(' ');
			if(firstSpace < 0) continue;

			var keyword = trimmed[..firstSpace];
			if(!ActionKeywords.TryGetValue(keyword, out var action)) continue;

			var rest = trimmed[(firstSpace + 1)..].TrimStart();

			if(action == RebaseAction.Exec)
			{
				entries.Add(new RebaseTodoEntry(action, hash: string.Empty, subject: rest));
				continue;
			}

			var secondSpace = rest.IndexOf(' ');
			string hash, subject;
			if(secondSpace < 0)
			{
				hash    = rest;
				subject = string.Empty;
			}
			else
			{
				hash    = rest[..secondSpace];
				subject = rest[(secondSpace + 1)..];
			}
			entries.Add(new RebaseTodoEntry(action, hash, subject));
		}
		return entries;
	}

	// git's todo parser does not strip BOM
	static readonly Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

	public static void Write(string filePath, IReadOnlyList<RebaseTodoEntry> entries)
	{
		using var writer = new StreamWriter(filePath, append: false, Utf8NoBom);
		foreach(var entry in entries)
		{
			var keyword = ActionToKeyword[entry.Action];
			if(entry.Action == RebaseAction.Exec)
			{
				writer.WriteLine($"{keyword} {entry.Subject}");
			}
			else
			{
				writer.WriteLine($"{keyword} {entry.Hash} {entry.Subject}");
			}
		}
	}
}
