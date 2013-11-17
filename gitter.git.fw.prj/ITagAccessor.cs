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
	using System.Threading;
	using System.Threading.Tasks;

	using gitter.Framework;

	/// <summary>Object which can perform various operations on git tags.</summary>
	public interface ITagAccessor
	{
		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		TagData QueryTag(QueryTagParameters parameters);

		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<TagData> QueryTagAsync(QueryTagParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag nema or object hash.</param>
		/// <returns>Tag message.</returns>
		string QueryTagMessage(string tag);

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag name or object hash.</param>
		/// <returns>Tag message.</returns>
		Task<string> QueryTagMessage(string tag, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<TagData> QueryTags(QueryTagsParameters parameters);

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task<IList<TagData>> QueryTagsAsync(QueryTagsParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CreateTag(CreateTagParameters parameters);

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task CreateTagAsync(CreateTagParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void DeleteTag(DeleteTagParameters parameters);

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task DeleteTagAsync(DeleteTagParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void VerifyTags(VerifyTagsParameters parameters);

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		Task VerifyTagsAsync(VerifyTagsParameters parameters, IProgress<OperationProgress> progress, CancellationToken cancellationToken);
	}
}
