namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Parameters for <see cref="IRepositoryAccessor.UnsetConfigValue"/> operation.</summary>
	public sealed class UnsetConfigValueParameters : BaseConfigParameters
	{
		/// <summary>Create <see cref="UnsetConfigValueParameters"/>.</summary>
		public UnsetConfigValueParameters()
		{
		}

		/// <summary>Create <see cref="UnsetConfigValueParameters"/>.</summary>
		/// <param name="parameterName">Parameter to unset.</param>
		public UnsetConfigValueParameters(string parameterName)
		{
			ParameterName = parameterName;
		}

		/// <summary>Parameter to unset.</summary>
		public string ParameterName { get; set; }
	}
}
