namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.AddConfigValue"/> operation.</summary>
	public sealed class AddConfigValueParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="AddConfigValueParameters"/>.</summary>
		public AddConfigValueParameters()
		{
		}

		/// <summary>Create <see cref="AddConfigValueParameters"/>.</summary>
		/// <param name="parameterName">Parameter name.</param>
		/// <param name="parameterValue">Parameter value.</param>
		public AddConfigValueParameters(string parameterName, string parameterValue)
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
