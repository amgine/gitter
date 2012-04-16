namespace gitter.Git.AccessLayer
{
	using System;

	using gitter.Framework;

	/// <summary>Information about <see cref="Submodule"/>.</summary>
	public sealed class SubmoduleData : INamedObject
	{
		#region Data

		private readonly string _name;
		private string _path;
		private string _url;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="SubmoduleData"/>.</summary>
		/// <param name="name">Submodule name.</param>
		public SubmoduleData(string name)
		{
			_name = name;
		}

		#endregion

		#region Properties

		/// <summary>Submodule name.</summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>Submodule path.</summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>Submodule URL.</summary>
		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		#endregion
	}
}
