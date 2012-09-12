namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Tag description.</summary>
	public sealed class TagData : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _sha1;
		private readonly TagType _tagType;

		#endregion

		#region .ctor

		public TagData(string name, string sha1, TagType tagType)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(sha1, "sha1");
			Verify.Argument.IsTrue(sha1.Length == 40, "sha1");

			_name = name;
			_sha1 = sha1;
			_tagType = tagType;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public string SHA1
		{
			get { return _sha1; }
		}

		public TagType TagType
		{
			get { return _tagType; }
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return _name;
		}
	}
}
