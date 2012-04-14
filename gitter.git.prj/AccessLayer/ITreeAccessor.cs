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
