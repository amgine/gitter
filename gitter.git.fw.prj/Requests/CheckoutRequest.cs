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

/// <summary>Parameters for <see cref="IRepositoryAccessor.Checkout"/> operation.</summary>
public sealed class CheckoutRequest
{
	/// <summary>Create <see cref="CheckoutRequest"/>.</summary>
	public CheckoutRequest()
	{
	}

	/// <summary>Create <see cref="CheckoutRequest"/>.</summary>
	/// <param name="revision">Revision to checkout.</param>
	public CheckoutRequest(string revision)
	{
		Revision = revision;
	}

	/// <summary>Create <see cref="CheckoutRequest"/>.</summary>
	/// <param name="revision">Revision to checkout.</param>
	/// <param name="force">Throw away local changes.</param>
	public CheckoutRequest(string revision, bool force)
	{
		Revision = revision;
		Force = force;
	}

	/// <summary>Revision to checkout.</summary>
	public string Revision { get; set; } = default!;

	/// <summary>Throw away local changes.</summary>
	public bool Force { get; set; }

	/// <summary>Merge local changes with target revision's tree.</summary>
	public bool Merge { get; set; }
}
