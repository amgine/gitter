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

	/// <summary>Object which can perform various operations on git tree object.</summary>
	public interface ITreeAccessor
	{
		/// <summary>Checkout files from tree object to working directory.</summary>
		/// <param name="parameters"><see cref="CheckoutFilesParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CheckoutFiles(CheckoutFilesParameters parameters);

		/// <summary>Get objects contained in a tree.</summary>
		/// <param name="parameters"><see cref="QueryTreeContentParameters"/>.</param>
		/// <returns>List of contained objects.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<TreeContentData> QueryTreeContent(QueryTreeContentParameters parameters);

		/// <summary>Queries the BLOB bytes.</summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns>Requested blob content.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		byte[] QueryBlobBytes(QueryBlobBytesParameters parameters);
	}
}
