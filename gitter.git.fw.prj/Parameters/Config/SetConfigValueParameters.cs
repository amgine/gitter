namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.SetConfigValue"/> operation.</summary>
	public sealed class SetConfigValueParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="SetConfigValueParameters"/>.</summary>
		public SetConfigValueParameters()
		{
		}

		/// <summary>Create <see cref="SetConfigValueParameters"/>.</summary>
		/// <param name="parameterName">Parameter name.</param>
		/// <param name="parameterValue">Parameter value.</param>
		public SetConfigValueParameters(string parameterName, string parameterValue)
		{
			ParameterName = parameterName;
			ParameterValue = parameterValue;
		}

		/// <summary>Parameter name.</summary>
		public string ParameterName { get; set; }

		/// <summary>Parameter value.</summary>
		public string ParameterValue { get; set; }
	}
}
