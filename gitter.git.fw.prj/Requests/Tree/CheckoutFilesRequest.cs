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

using gitter.Framework;

/// <summary>Parameters for <see cref="IRepositoryAccessor.CheckoutFiles"/> operation.</summary>
public sealed class CheckoutFilesRequest
{
	/// <summary>Create <see cref="CheckoutFilesRequest"/>.</summary>
	public CheckoutFilesRequest()
	{
	}

	/// <summary>Create <see cref="CheckoutFilesRequest"/>.</summary>
	/// <param name="revision">Tree or commit referencing a tree.</param>
	/// <param name="paths">Paths to checkout.</param>
	public CheckoutFilesRequest(string revision, Many<string> paths)
	{
		Revision = revision;
		Paths = paths;
	}

	/// <summary>Create <see cref="CheckoutFilesRequest"/>.</summary>
	/// <param name="paths">Paths to checkout.</param>
	public CheckoutFilesRequest(Many<string> paths)
	{
		Paths = paths;
	}

	/// <summary>Create <see cref="CheckoutFilesRequest"/>.</summary>
	/// <param name="revision">Tree or commit referencing a tree.</param>
	/// <param name="path">Path to checkout.</param>
	public CheckoutFilesRequest(string revision, string path)
	{
		Revision = revision;
		Paths = path;
	}

	/// <summary>Create <see cref="CheckoutFilesRequest"/>.</summary>
	/// <param name="path">Path to checkout.</param>
	public CheckoutFilesRequest(string path)
	{
		Paths = path;
	}

	/// <summary>Tree or commit referencing a tree.</summary>
	public string Revision { get; set; } = default!;

	/// <summary>Paths to checkout.</summary>
	public Many<string> Paths { get; set; }

	/// <summary>Checkout mode.</summary>
	public CheckoutFileMode Mode { get; set; }
}
