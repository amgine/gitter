namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.QueryConfigParamter"/> operation.</summary>
	public sealed class QueryConfigParameterParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="QueryConfigParameterParameters"/>.</summary>
		public QueryConfigParameterParameters()
		{
		}

		/// <summary>Create <see cref="QueryConfigParameterParameters"/>.</summary>
		/// <param name="parameterName">Parameter to query.</param>
		public QueryConfigParameterParameters(string parameterName)
		{
			ParameterName = parameterName;
		}

		/// <summary>Create <see cref="QueryConfigParameterParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		/// <param name="fileName">Config file name.</param>
		/// <param name="parameterName">Parameter to query.</param>
		public QueryConfigParameterParameters(ConfigFile configFile, string fileName, string parameterName)
			: base(configFile, fileName)
		{
			ParameterName = parameterName;
		}

		/// <summary>Create <see cref="QueryConfigParameterParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		/// <param name="parameterName">Parameter to query.</param>
		public QueryConfigParameterParameters(ConfigFile configFile, string parameterName)
			: base(configFile)
		{
			ParameterName = parameterName;
		}

		/// <summary>Create <see cref="QueryConfigParameterParameters"/>.</summary>
		/// <param name="fileName">Config file name.</param>
		/// <param name="parameterName">Parameter to query.</param>
		public QueryConfigParameterParameters(string fileName, string parameterName)
			: base(fileName)
		{
			ParameterName = parameterName;
		}

		/// <summary>Parameter to query.</summary>
		public string ParameterName { get; set; }
	}
}
