namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="ITagAccessor.CreateTag()"/> operation.</summary>
	public sealed class CreateTagParameters
	{
		/// <summary>Create <see cref="CreateTagParameters"/>.</summary>
		public CreateTagParameters()
		{
		}

		/// <summary>Create <see cref="CreateTagParameters"/>.</summary>
		/// <param name="tagName">Name of the tag to create.</param>
		/// <param name="taggedObject">Object to tag.</param>
		public CreateTagParameters(string tagName, string taggedObject)
		{
			TagName = tagName;
			TaggedObject = taggedObject;
			TagType = TagType.Lightweight;
		}

		/// <summary>Create <see cref="CreateTagParameters"/>.</summary>
		/// <param name="tagName">Name of the tag to create.</param>
		/// <param name="taggedObject">Object to tag.</param>
		/// <param name="message">Tag message.</param>
		public CreateTagParameters(string tagName, string taggedObject, string message)
		{
			TagName = tagName;
			TaggedObject = taggedObject;
			TagType = TagType.Annotated;
			Message = message;
		}

		/// <summary>Create <see cref="CreateTagParameters"/>.</summary>
		/// <param name="tagName">Name of the tag to create.</param>
		/// <param name="taggedObject">Object to tag.</param>
		/// <param name="message">Tag message.</param>
		/// <param name="signed">Create signed tag.</param>
		public CreateTagParameters(string tagName, string taggedObject, string message, bool signed)
		{
			TagName = tagName;
			TaggedObject = taggedObject;
			TagType = TagType.Annotated;
			Message = message;
			Signed = signed;
		}

		/// <summary>Create <see cref="CreateTagParameters"/>.</summary>
		/// <param name="tagName">Name of the tag to create.</param>
		/// <param name="taggedObject">Object to tag.</param>
		/// <param name="message">Tag message.</param>
		/// <param name="keyId">Signing key ID.</param>
		public CreateTagParameters(string tagName, string taggedObject, string message, string keyId)
		{
			TagName = tagName;
			TaggedObject = taggedObject;
			TagType = TagType.Annotated;
			Message = message;
			Signed = true;
			KeyId = keyId;
		}

		/// <summary>Name of the tag to create.</summary>
		public string TagName { get; set; }

		/// <summary>Object to tag.</summary>
		public string TaggedObject { get; set; }

		/// <summary>Tag type.</summary>
		public TagType TagType { get; set; }

		/// <summary>Tag message.</summary>
		public string Message { get; set; }

		/// <summary>Tag message file.</summary>
		public string MessageFile { get; set; }

		/// <summary>Sign tag.</summary>
		public bool Signed { get; set; }

		/// <summary>Key ID for signing.</summary>
		public string KeyId { get; set; }

		/// <summary>Force-overwrite existing tag.</summary>
		public bool Force { get; set; }
	}
}
