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

namespace gitter.Git.AccessLayer;

using System;
using System.Collections.Generic;

using gitter.Framework;

/// <summary>Parameters for <see cref="IRepositoryAccessor.CherryPick"/> operation.</summary>
public sealed class CherryPickRequest
{
	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	public CherryPickRequest()
	{
	}

	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	public CherryPickRequest(CherryPickControl control)
	{
		Control = control;
	}

	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	/// <param name="revision">Revision to cherry-pick.</param>
	public CherryPickRequest(string revision)
	{
		Revisions = revision;
	}

	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	/// <param name="revisions">Revisions to cherry-pick.</param>
	public CherryPickRequest(Many<string> revisions)
	{
		Revisions = new(revisions);
	}

	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	/// <param name="revision">Revision to cherry-pick.</param>
	/// <param name="noCommit">>Don't create commit.</param>
	public CherryPickRequest(string revision, bool noCommit)
	{
		Revisions = revision;
		NoCommit  = noCommit;
	}

	/// <summary>Create <see cref="CherryPickRequest"/>.</summary>
	/// <param name="revisions">Revisions to cherry-pick.</param>
	/// <param name="noCommit">>Don't create commit.</param>
	public CherryPickRequest(Many<string> revisions, bool noCommit)
	{
		Revisions = new(revisions);
		NoCommit  = noCommit;
	}

	public CherryPickControl? Control { get; set; }

	/// <summary>Revisions to cherry-pick.</summary>
	public Many<string> Revisions { get; set; }

	/// <summary>Don't create commit.</summary>
	public bool NoCommit { get; set; }

	public bool SignOff { get; set; }

	public int Mainline { get; set; }

	public bool AllowEmpty { get; set; }

	public bool AllowEmptyMessage { get; set; }

	public bool FastForward { get; set; }

	public bool KeepRedundantCommits { get; set; }
}
