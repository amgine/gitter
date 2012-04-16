namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git tags.</summary>
	public interface ITagAccessor
	{
		/// <summary>Check if tag exists and get its position.</summary>
		/// <param name="parameters"><see cref="QueryTagParameters"/>.</param>
		/// <returns><see cref="TagData"/> or null, if requested tag doesn't exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		TagData QueryTag(QueryTagParameters parameters);

		/// <summary>Query tag message.</summary>
		/// <param name="tag">Tag nema or object hash.</param>
		/// <returns>Tag message.</returns>
		string QueryTagMessage(string tag);

		/// <summary>Query tag list.</summary>
		/// <param name="parameters"><see cref="QueryTagsParameters"/>.</param>
		/// <returns>List of requested tags.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<TagData> QueryTags(QueryTagsParameters parameters);

		/// <summary>Create new tag object.</summary>
		/// <param name="parameters"><see cref="CreateTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void CreateTag(CreateTagParameters parameters);

		/// <summary>Delete tag.</summary>
		/// <param name="parameters"><see cref="DeleteTagParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void DeleteTag(DeleteTagParameters parameters);

		/// <summary>Verify tags GPG signatures.</summary>
		/// <param name="parameters"><see cref="VerifyTagsParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void VerifyTags(VerifyTagsParameters parameters);
	}
}
