namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;
	
	/// <summary>Parameters for <see cref="ITagAccessor.VerifyTags()"/> operation.</summary>
	public sealed class VerifyTagsParameters
	{
		/// <summary>Create <see cref="VerifyTagsParameters"/>.</summary>
		public VerifyTagsParameters()
		{
		}

		/// <summary>Create <see cref="VerifyTagsParameters"/>.</summary>
		public VerifyTagsParameters(string tagName)
		{
			TagNames = new[] { tagName };
		}

		/// <summary>Create <see cref="VerifyTagsParameters"/>.</summary>
		public VerifyTagsParameters(IList<string> tagNames)
		{
			TagNames = tagNames;
		}

		/// <summary>Tags to verify.</summary>
		public IList<string> TagNames { get; set; }
	}
}
