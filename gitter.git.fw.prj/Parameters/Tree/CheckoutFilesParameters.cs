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

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Parameters for <see cref="ITreeAccessor.CheckoutFiles"/> operation.</summary>
	public sealed class CheckoutFilesParameters
	{
		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		public CheckoutFilesParameters()
		{
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="revision">Tree or commit referencing a tree.</param>
		/// <param name="paths">Paths to checkout.</param>
		public CheckoutFilesParameters(string revision, IList<string> paths)
		{
			Revision = revision;
			Paths = paths;
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="paths">Paths to checkout.</param>
		public CheckoutFilesParameters(IList<string> paths)
		{
			Paths = paths;
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="revision">Tree or commit referencing a tree.</param>
		/// <param name="path">Path to checkout.</param>
		public CheckoutFilesParameters(string revision, string path)
		{
			Revision = revision;
			Paths = new[] { path };
		}

		/// <summary>Create <see cref="CheckoutFilesParameters"/>.</summary>
		/// <param name="path">Path to checkout.</param>
		public CheckoutFilesParameters(string path)
		{
			Paths = new[] { path };
		}

		/// <summary>Tree or commit referencing a tree.</summary>
		public string Revision { get; set; }

		/// <summary>Paths to checkout.</summary>
		public IList<string> Paths { get; set; }

		/// <summary>Checkout mode.</summary>
		public CheckoutFileMode Mode { get; set; }
	}
}
