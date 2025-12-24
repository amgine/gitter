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

using System;
using System.Collections.Generic;
using System.Globalization;

/// <summary>Show commit logs.</summary>
public sealed class LogCommand : Command
{
	const string LogCommandName = "log";

	public static class KnownArguments
	{
		public static ICommandArgument All { get; }
			= new CommandFlag("--all");

		public static ICommandArgument Not { get; }
			= new CommandFlag("--not");

		public static ICommandArgument Branches(string pattern)
			=> new CommandParameterValue("--branches", pattern, '=');

		public static ICommandArgument Tags(string pattern)
			=> new CommandParameterValue("--tags", pattern, '=');

		public static ICommandArgument Glob(string pattern)
			=> new CommandParameterValue("--glob", pattern, '=');

		public static ICommandArgument Remotes(string pattern)
			=> new CommandParameterValue("--remotes", pattern, '=');

		public static ICommandArgument Author(string pattern)
			=> new CommandParameterValue("--author", pattern, '=');

		public static ICommandArgument SimplifyByDecoration { get; }
			= new CommandFlag("--simplify-by-decoration");

		public static ICommandArgument Committer(string pattern)
			=> new CommandParameterValue("--committer", pattern, '=');

		public static ICommandArgument Grep(string pattern)
			=> new CommandParameterValue("--grep", pattern, '=');

		public static ICommandArgument RegexpIgnoreCase { get; }
			= new CommandFlag("--regexp-ignore-case");

		public static ICommandArgument ExtendedRegexp { get; }
			= new CommandFlag("--extended-regexp");

		public static ICommandArgument FixedStrings { get; }
			= new CommandFlag("--fixed-strings");

		public static ICommandArgument AllMatch { get; }
			= new CommandFlag("--all-match");

		/// <summary>Follow only the first parent commit upon seeing a merge commit.</summary>
		public static ICommandArgument FirstParent { get; }
			= new CommandFlag("--first-parent");

		/// <summary>Print also the parents of the commit (in the form "commit parent..."). Also enables parent rewriting.</summary>
		public static ICommandArgument Parents { get; }
			= new CommandFlag("--parents");

		/// <summary>Print only merge commits.</summary>
		public static ICommandArgument Merges { get; }
			= new CommandFlag("--merges");

		/// <summary>Do not print commits with more than one parent.</summary>
		public static ICommandArgument NoMerges { get; }
			= new CommandFlag("--no-merges");

		public static ICommandArgument Graph { get; }
			= new CommandFlag("--graph");

		public static ICommandArgument WalkReflogs { get; }
			= new CommandFlag("--walk-reflogs");

		/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
		public static ICommandArgument Follow { get; }
			= new CommandFlag("--follow");

		/// <summary>Stop when a given path disappears from the tree.</summary>
		public static ICommandArgument RemoveEmpty { get;}
			= new CommandFlag("--remove-empty");

		private static readonly CommandParameterValue _maxCount1 = new("--max-count", "1");

		public static ICommandArgument MaxCount(int limit)
			=> limit switch
			{
				1 => _maxCount1,
				_ => new CommandParameterIntValue("--max-count", limit),
			};

		public static ICommandArgument Skip(int skip)
			=> new CommandParameterIntValue("--skip", skip);

		public static ICommandArgument Format(string format)
			=> new CommandParameterValue("--format", "format:\"" + format + "\"");

		public static ICommandArgument FormatRaw { get; }
			= new CommandParameterValue("--format", "raw");

		public static ICommandArgument TFormat(string format)
			=> new CommandParameterValue("--format", "tformat:\"" + format + "\"");

		public static ICommandArgument OneLine { get; }
			= new CommandFlag("--oneline");

		public static ICommandArgument Reverse { get; }
			= new CommandFlag("--reverse");

		public static ICommandArgument TopoOrder { get; }
			= new CommandFlag("--topo-order");

		public static ICommandArgument DateOrder { get; }
			= new CommandFlag("--date-order");

		public static ICommandArgument Since(DateTime dateTime)
			=> new CommandParameterValue("--since", dateTime.ToString(CultureInfo.InvariantCulture));

		public static ICommandArgument Until(DateTime dateTime)
			=> new CommandParameterValue("--until", dateTime.ToString(CultureInfo.InvariantCulture));

		public static ICommandArgument CombinedDiff { get; }
			= new CommandFlag("-c");

		public static ICommandArgument DenseCombinedDiff { get; }
			= new CommandFlag("--cc");

		public static ICommandArgument NullTerminate { get; } = new CommandFlag("-z");

		public static ICommandArgument NoMoreOptions => CommandFlag.NoMoreOptions;
	}

	public sealed class Builder()
		: CommandBuilderBase(LogCommandName)
		, ICommandBuilder
		, IDiffCommandBuilder
		, ICommandBuilderSpecifyRefs
		, ICommandBuilderSpecifyPaths
	{
		public void All()
			=> AddArgument(KnownArguments.All);

		public void Not()
			=> AddArgument(KnownArguments.Not);

		public void Branches(string pattern)
			=> AddArgument(KnownArguments.Branches(pattern));

		public void Tags(string pattern)
			=> AddArgument(KnownArguments.Tags(pattern));

		public void Glob(string pattern)
			=> AddArgument(KnownArguments.Glob(pattern));

		public void Remotes(string pattern)
			=> AddArgument(KnownArguments.Remotes(pattern));

		public void Author(string pattern)
			=> AddArgument(KnownArguments.Author(pattern));

		public void SimplifyByDecoration()
			=> AddArgument(KnownArguments.SimplifyByDecoration);

		public void Committer(string pattern)
			=> AddArgument(KnownArguments.Committer(pattern));

		public void Grep(string pattern)
			=> AddArgument(KnownArguments.Grep(pattern));

		public void RegexpIgnoreCase()
			=> AddArgument(KnownArguments.RegexpIgnoreCase);

		public void ExtendedRegexp()
			=> AddArgument(KnownArguments.ExtendedRegexp);

		public void FixedStrings()
			=> AddArgument(KnownArguments.FixedStrings);

		public void AllMatch()
			=> AddArgument(KnownArguments.AllMatch);

		/// <summary>Follow only the first parent commit upon seeing a merge commit.</summary>
		public void FirstParent()
			=> AddArgument(KnownArguments.FirstParent);

		/// <summary>Print also the parents of the commit (in the form "commit parent..."). Also enables parent rewriting.</summary>
		public void Parents()
			=> AddArgument(KnownArguments.Parents);

		/// <summary>Print only merge commits.</summary>
		public void Merges()
			=> AddArgument(KnownArguments.Merges);

		/// <summary>Do not print commits with more than one parent.</summary>
		public void NoMerges()
			=> AddArgument(KnownArguments.NoMerges);

		public void Graph()
			=> AddArgument(KnownArguments.Graph);

		public void WalkReflogs()
			=> AddArgument(KnownArguments.WalkReflogs);

		/// <summary>Continue listing the history of a file beyond renames (works only for a single file).</summary>
		public void Follow()
			=> AddArgument(KnownArguments.Follow);

		/// <summary>Stop when a given path disappears from the tree.</summary>
		public void RemoveEmpty()
			=> AddArgument(KnownArguments.RemoveEmpty);

		public void MaxCount(int limit)
		{
			if(limit <= 0) return;
			AddArgument(KnownArguments.MaxCount(limit));
		}

		public void Skip(int skip)
		{
			if(skip <= 0) return;
			AddArgument(KnownArguments.Skip(skip));
		}

		public void Format(string format)
			=> AddArgument(KnownArguments.Format(format));

		public void FormatRaw()
			=> AddArgument(KnownArguments.FormatRaw);

		public void TFormat(string format)
			=> AddArgument(KnownArguments.TFormat(format));

		public void Since(DateTime dateTime)
			=> AddArgument(KnownArguments.Since(dateTime));

		public void Until(DateTime dateTime)
			=> AddArgument(KnownArguments.Until(dateTime));

		public void OneLine()
			=> AddArgument(KnownArguments.OneLine);

		public void Reverse()
			=> AddArgument(KnownArguments.Reverse);

		public void TopoOrder()
			=> AddArgument(KnownArguments.TopoOrder);

		public void DateOrder()
			=> AddArgument(KnownArguments.DateOrder);

		public void CombinedDiff(bool dense = false)
			=> AddArgument(dense ? KnownArguments.DenseCombinedDiff : KnownArguments.CombinedDiff);

		public void Order(RevisionQueryOrder order)
		{
			switch(order)
			{
				case RevisionQueryOrder.Default:                break;
				case RevisionQueryOrder.TopoOrder: TopoOrder(); break;
				case RevisionQueryOrder.DateOrder: DateOrder(); break;
				default: throw new ArgumentException($"Unknown order: {order}", nameof(order));
			}
		}

		public void NullTerminate()
			=> AddArgument(KnownArguments.NullTerminate);

		public void NoMoreOptions()
			=> AddArgument(KnownArguments.NoMoreOptions);
	}

	public LogCommand()
		: base(LogCommandName)
	{
	}

	public LogCommand(params ICommandArgument[] args)
		: base(LogCommandName, args)
	{
	}

	public LogCommand(IEnumerable<ICommandArgument> args)
		: base(LogCommandName, args)
	{
	}
}
