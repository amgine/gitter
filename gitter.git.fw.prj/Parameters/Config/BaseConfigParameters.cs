namespace gitter.Git.AccessLayer
{
	using System;

	/// <summary>Base class for all config-related operation parameters.</summary>
	public abstract class BaseConfigParameters
	{
		/// <summary>Create <see cref="BaseConfigParameters"/>.</summary>
		protected BaseConfigParameters()
		{
		}

		/// <summary>Create <see cref="BaseConfigParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		/// <param name="fileName">Config file name.</param>
		protected BaseConfigParameters(ConfigFile configFile, string fileName)
		{
			ConfigFile = configFile;
			FileName = fileName;
		}

		/// <summary>Create <see cref="BaseConfigParameters"/>.</summary>
		/// <param name="configFile">Config file type.</param>
		protected BaseConfigParameters(ConfigFile configFile)
		{
			ConfigFile = configFile;
		}

		/// <summary>Create <see cref="BaseConfigParameters"/>.</summary>
		/// <param name="fileName">Config file name.</param>
		protected BaseConfigParameters(string fileName)
		{
			ConfigFile = Git.ConfigFile.Other;
			FileName = fileName;
		}

		/// <summary>Type of file to query.</summary>
		public ConfigFile ConfigFile { get; set; }

		/// <summary>Name of the file to query.</summary>
		public string FileName { get; set; }
	}
}
