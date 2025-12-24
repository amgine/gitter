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
using System.Globalization;

/// <summary>Show changes between commits, commit and working tree, etc.</summary>
public sealed class DiffCommand : Command
{
	const string DiffCommandName = "diff";

	public static class KnownArguments
	{
		public static ICommandArgument FullIndex { get; }
			= new CommandFlag("--full-index");

		public static ICommandArgument Binary { get; }
			= new CommandFlag("--binary");

		public static ICommandArgument NoPrefix { get; }
			= new CommandFlag("--no-prefix");

		public static ICommandArgument Cached { get; }
			= new CommandFlag("--cached");

		public static ICommandArgument Patch { get; }
			= new CommandFlag("-p");

		public static ICommandArgument Raw { get; }
			= new CommandFlag("--raw");

		public static ICommandArgument Patience { get; }
			= new CommandFlag("--patience");

		public static ICommandArgument Stat { get; }
			= new CommandFlag("--stat");

		public static ICommandArgument NumStat { get; }
			= new CommandFlag("--numstat");

		public static ICommandArgument ShortStat { get; }
			= new CommandFlag("--shortstat");

		public static ICommandArgument NameOnly { get; }
			= new CommandFlag("--name-only");

		public static ICommandArgument NameStatus { get; }
			= new CommandFlag("--name-status");

		public static ICommandArgument IgnoreSpaceChange { get; }
			= new CommandFlag("--ignore-space-change");

		public static ICommandArgument IgnoreSpaceAtEOL { get; }
			= new CommandFlag("--ignore-space-at-eol");

		public static ICommandArgument IgnoreAllSpace { get; }
			= new CommandFlag("--ignore-all-space");

		public static ICommandArgument NoRenames { get; }
			= new CommandFlag("--no-renames");

		public static ICommandArgument PatchWithRaw { get; }
			= new CommandFlag("--patch-with-raw");

		public static ICommandArgument Unified(int n)
			=> new CommandParameterValue("--unified", n.ToString(CultureInfo.InvariantCulture));

		public static ICommandArgument Check { get; }
			= new CommandFlag("--check");

		public static ICommandArgument NoColor { get; }
			= new CommandFlag("--no-color");

		public static ICommandArgument SwapInputs { get; }
			= new CommandFlag("-R");

		public static ICommandArgument TextConv { get; }
			= new CommandFlag("--textconv");

		public static ICommandArgument NoTextConv { get; }
			= new CommandFlag("--no-textconv");

		public static ICommandArgument ExtDiff { get; }
			= new CommandFlag("--ext-diff");

		public static ICommandArgument NoExtDiff { get; }
			= new CommandFlag("--no-ext-diff");

		public static ICommandArgument Text { get; }
			= new CommandFlag("--text");

		public static ICommandArgument FindRenames()
			=> new CommandFlag("--find-renames");

		public static ICommandArgument FindRenames(double similarity)
			=> new CommandParameterValue("--find-renames", similarity.ToString("G", CultureInfo.InvariantCulture).Substring(2), '=');

		public static ICommandArgument FindCopies()
			=> new CommandFlag("--find-copies");

		public static ICommandArgument FindCopies(double similarity)
			=> new CommandParameterValue("--find-copies", similarity.ToString("G", CultureInfo.InvariantCulture).Substring(2), '=');

		public static ICommandArgument NoMoreOptions
			=> CommandFlag.NoMoreOptions;

		public static ICommandArgument NullTerminate { get; }
			= new CommandFlag("-z");
	}

	public sealed class Builder()
		: CommandBuilderBase(DiffCommandName)
		, IDiffCommandBuilder
		, ICommandBuilderSpecifyRefs
		, ICommandBuilderSpecifyPaths
	{
		public void NullTerminate()
			=> AddArgument(KnownArguments.NullTerminate);
	}

	public DiffCommand()
		: base(DiffCommandName)
	{
	}

	public DiffCommand(params ICommandArgument[] args)
		: base(DiffCommandName, args)
	{
	}

	public DiffCommand(IList<ICommandArgument> args)
		: base(DiffCommandName, args)
	{
	}
}

public static class DiffCommandBuilderExtensions
{
	public static void FullIndex<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.FullIndex);

	public static void Binary<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Binary);

	public static void NoPrefix<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NoPrefix);

	public static void Cached<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Cached);

	public static void Patch<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Patch);

	public static void Raw<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Raw);

	public static void Patience<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Patience);

	public static void Stat<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Stat);

	public static void NumStat<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NumStat);

	public static void ShortStat<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.ShortStat);

	public static void NameOnly<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NameOnly);

	public static void NameStatus<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NameStatus);

	public static void IgnoreSpaceChange<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.IgnoreSpaceChange);

	public static void IgnoreSpaceAtEOL<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.IgnoreSpaceAtEOL);

	public static void IgnoreAllSpace<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.IgnoreAllSpace);

	public static void NoRenames<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NoRenames);

	public static void PatchWithRaw<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.PatchWithRaw);

	public static void Unified<T>(this T builder, int n) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Unified(n));

	public static void Check<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Check);

	public static void NoColor<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NoColor);

	public static void SwapInputs<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.SwapInputs);

	public static void TextConv<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.TextConv);

	public static void NoTextConv<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NoTextConv);

	public static void ExtDiff<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.ExtDiff);

	public static void NoExtDiff<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.NoExtDiff);

	public static void Text<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.Text);

	public static void FindRenames<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.FindRenames());

	public static void FindRenames<T>(this T builder, double similarity) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.FindRenames(similarity));

	public static void FindCopies<T>(this T builder) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.FindCopies());

	public static void FindCopies<T>(this T builder, double similarity) where T : IDiffCommandBuilder
		=> builder.AddArgument(DiffCommand.KnownArguments.FindCopies(similarity));
}
