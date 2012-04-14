namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="ITagAccessor.DeleteTag()"/> operation.</summary>
	public sealed class DeleteTagParameters
	{
		/// <summary>Create <see cref="DeleteTagParameters"/>.</summary>
		public DeleteTagParameters()
		{
		}

		/// <summary>Create <see cref="DeleteTagParameters"/>.</summary>
		/// <param name="tagName">Name of tag to delete.</param>
		public DeleteTagParameters(string tagName)
		{
			TagName = tagName;
		}


		/// <param name="tagName">Name of tag to delete.</param>
		public string TagName { get; set; }
	}
}
