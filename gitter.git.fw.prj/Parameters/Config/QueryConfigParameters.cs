namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryConfig"/> operation.</summary>
	public sealed class QueryConfigParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="QueryConfigParameters"/>.</summary>
		public QueryConfigParameters()
		{
		}

		/// <summary>Create <see cref="QueryConfigParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		/// <param name="fileName">Config file name.</param>
		public QueryConfigParameters(ConfigFile configFile, string fileName)
			: base(configFile, fileName)
		{
		}

		/// <summary>Create <see cref="QueryConfigParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		public QueryConfigParameters(ConfigFile configFile)
			: base(configFile)
		{
		}

		/// <summary>Create <see cref="QueryConfigParameters"/>.</summary>
		/// <param name="fileName">Config file name.</param>
		public QueryConfigParameters(string fileName)
			: base(fileName)
		{
		}
	}
}
