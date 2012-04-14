namespace gitter.Git
{
	using System;

	using Resources = gitter.Git.Properties.Resources;

	/// <summary>git tag object.</summary>
	public sealed class Tag : Reference
	{
		#region Data

		/// <summary>Tag type.</summary>
		private TagType _type;
		/// <summary>Annotated tag message.</summary>
		private string _message;

		#endregion

		#region .ctor

		/// <summary>Initializes a new instance of the <see cref="Tag"/> class.</summary>
		/// <param name="repository">Host repository.</param>
		/// <param name="name">Tag name.</param>
		/// <param name="pointer">Commit which is pointed by tag.</param>
		/// <param name="type">The type.</param>
		internal Tag(Repository repository, string name, IRevisionPointer pointer, TagType type)
			: base(repository, name, pointer)
		{
			_type = type;
		}

		#endregion

		#region Properties

		/// <summary><see cref="ReferenceType"/>.</summary>
		/// <value><see cref="ReferenceType.Tag"/>.</value>
		public override ReferenceType Type
		{
			get { return ReferenceType.Tag; }
		}

		/// <summary>Gets the full tag name.</summary>
		/// <value>Full tag name.</value>
		public override string FullName
		{
			get { return GitConstants.TagPrefix + Name; }
		}

		/// <summary>Gets or sets the type of this tag.</summary>
		/// <value>Type of this tag.</value>
		public TagType TagType
		{
			get { return _type; }
			internal set { _type = value; }
		}

		/// <summary>Gets the message of annotated tag.</summary>
		/// <value>Message of annotated tag.</value>
		public string Message
		{
			get
			{
				if(_type == Git.TagType.Lightweight)
				{
					return null;
				}
				else
				{
					if(_message == null)
					{
						try
						{
							_message = Repository.Accessor.QueryTagMessage(FullName);
						}
						catch
						{
							_message = string.Empty;
						}
					}
					return _message;
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>Filter <see cref="IRevisionPointer"/> to types supported by this <see cref="Reference"/>.</summary>
		/// <param name="pointer">Raw pointer.</param>
		/// <returns>Valid pointer.</returns>
		protected override IRevisionPointer PrepareInputPointer(IRevisionPointer pointer)
		{
			return pointer.Dereference();
		}

		/// <summary>Deletes this <see cref="Tag"/>.</summary>
		public void Delete()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Tag"));
			}

			#endregion

			Repository.Refs.Tags.Delete(this);
		}

		/// <summary>Refreshes this instance's cached data.</summary>
		public void Refresh()
		{
			#region validate state

			if(IsDeleted)
			{
				throw new InvalidOperationException(string.Format(
					Resources.ExcObjectIsDeleted, "Tag"));
			}

			#endregion

			Repository.Refs.Tags.Refresh(this);
		}

		#endregion
	}
}
