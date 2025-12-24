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

/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
/// <remarks>
/// <code>
/// <![CDATA[
/// git init
///   [-q | --quiet]
///   [--bare]
///   [--template=<template-directory>]
///   [--separate-git-dir<git-dir>]
///   [--object-format=<format>]
///   [--ref-format=<format>]
///   [-b <branch-name> | --initial-branch=<branch-name>]
///   [--shared[=<permissions>]]
///   [<directory>]
/// ]]>
/// </code>
/// </remarks>
public sealed class InitCommand : Command
{
	const string InitCommandName = @"init";

	/*
	[-q | --quiet] [--bare] [--template=<template-directory>]
	[--separate-git-dir <git-dir>]
	[--object-format=<format>]
	[--ref-format=<format>]
	[-b <branch-name> | --initial-branch=<branch-name>]
	[--shared[=<permissions>]] [<directory>]	 
	 */

	public static class KnownArguments
	{
		public static ICommandArgument Template(string template)
			=> new CommandParameterPathValue("--template", template, '=');

		public static ICommandArgument Shared(string permissions)
			=> new CommandParameterValue("--shared", permissions, '=');

		public static ICommandArgument SeparateGitDirectory(string directory)
			=> new CommandParameterPathValue("--separate-git-dir", directory);

		public static ICommandArgument InitialBranch(string branchName)
			=> new CommandParameterValue("--initial-branch", branchName, '=');

		public static ICommandArgument ObjectFormat(string format)
			=> new CommandParameterValue("--object-format", format, '=');

		public static ICommandArgument RefFormat(string format)
			=> new CommandParameterValue("--ref-format", format, '=');

		public static ICommandArgument Bare { get; } = new CommandFlag("--bare");

		public static ICommandArgument Quiet { get; } = CommandFlag.Quiet;
	}

	public sealed class Builder() : CommandBuilderBase(InitCommandName)
	{
		/// <summary>Specify the directory from which templates will be used.</summary>
		public void Template(string template)
			=> AddArgument(KnownArguments.Template(template));

		public void Shared(string permissions)
			=> AddArgument(KnownArguments.Shared(permissions));

		/// <summary>
		/// Instead of initializing the repository as a directory to either <c>$GIT_DIR</c> or
		/// <c>./.git/</c>, create a text file there containing the path to the actual repository.<br/>
		/// This file acts as a filesystem-agnostic Git symbolic link to the repository.
		/// </summary>
		/// <remarks>
		/// If this is a reinitialization, the repository will be moved to the specified path.
		/// </remarks>
		public void SeparateGitDirectory(string directory)
			=> AddArgument(KnownArguments.SeparateGitDirectory(directory));

		/// <summary>
		/// Use <paramref name="branchName"/> for the initial branch in the newly created repository.
		/// </summary>
		public void InitialBranch(string branchName)
			=> AddArgument(KnownArguments.InitialBranch(branchName));

		/// <summary>
		/// Specify the given ref storage <paramref name="format"/> for the repository.
		/// </summary>
		/// <param name="format">
		/// <c>files</c> for loose files with packed-refs.This is the default.<br/>
		/// <c>reftable</c> for the reftable format. This format is experimental and its internals are subject to change.
		/// </param>
		public void RefFormat(string format)
			=> AddArgument(KnownArguments.RefFormat(format));

		public void Bare()
			=> AddArgument(KnownArguments.Bare);

		public void Quiet()
			=> AddArgument(KnownArguments.Quiet);
	}

	public InitCommand()
		: base(InitCommandName)
	{
	}

	public InitCommand(params ICommandArgument[] args)
		: base(InitCommandName, args)
	{
	}

	public InitCommand(IList<ICommandArgument> args)
		: base(InitCommandName, args)
	{
	}
}
