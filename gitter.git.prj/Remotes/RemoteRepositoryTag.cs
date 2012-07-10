namespace gitter.Git
{
	using System;
	using System.Collections.Generic;

	/// <summary>Represents a tag on remote repository.</summary>
	public sealed class RemoteRepositoryTag : BaseRemoteReference
	{
		private TagType _tagType;

		internal RemoteRepositoryTag(RemoteReferencesCollection refs, string name, TagType type, string hash)
			: base(refs, name, hash)
		{
			_tagType = type;
		}

		public TagType TagType
		{
			get { return _tagType; }
			internal set { _tagType = value; }
		}

		protected override void DeleteCore()
		{
			References.RemoveTag(this);
		}

		public override ReferenceType ReferenceType
		{
			get { return ReferenceType.Tag; }
		}
	}
}
