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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>In-progress interactive rebase session.</summary>
public interface IInteractiveRebaseSession
{
	/// <summary>Commits parsed from the rebase todo file, in execution order.</summary>
	IReadOnlyList<RebaseTodoEntry> Entries { get; }

	/// <summary>Path to the rebase todo file.</summary>
	string TodoFilePath { get; }

	/// <summary>Write modified plan, release the fake editor and wait for git to finish.</summary>
	Task<RebaseResult> CommitPlanAsync(IReadOnlyList<RebaseTodoEntry> modified, CancellationToken cancellationToken = default);

	/// <summary>Kill git process without writing the plan.</summary>
	Task AbortAsync(CancellationToken cancellationToken = default);
}

/// <summary>Outcome of an interactive rebase after the plan was committed.</summary>
public sealed class RebaseResult
{
	public RebaseResult(int exitCode, string stderr)
	{
		ExitCode = exitCode;
		Stderr   = stderr ?? string.Empty;
	}

	public int    ExitCode { get; }
	public string Stderr   { get; }
	public bool   Success  => ExitCode == 0;
}
