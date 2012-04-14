namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	partial class GlobalCLI : IConfigAccessor
	{
		/// <summary>Query config parameter.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameterParameters"/>.</param>
		/// <returns><see cref="ConfigParameterData"/> for requested parameter or null if parameter does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public ConfigParameterData QueryConfigParameter(QueryConfigParameterParameters parameters)
		{
			return GitConfigHelper.QueryConfigParameter(_executor, parameters);
		}

		/// <summary>Query configuration parameter list.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameters"/>.</param>
		/// <returns>List of requested parameters.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public IList<ConfigParameterData> QueryConfig(QueryConfigParameters parameters)
		{
			return GitConfigHelper.QueryConfig(_executor, parameters);
		}

		/// <summary>Add config value.</summary>
		/// <param name="parameters"><see cref="AddConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void AddConfigValue(AddConfigValueParameters parameters)
		{
			GitConfigHelper.AddConfigValue(_executor, parameters);
		}

		/// <summary>Set config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void SetConfigValue(SetConfigValueParameters parameters)
		{
			GitConfigHelper.SetConfigValue(_executor, parameters);
		}

		/// <summary>Unset config parameter value.</summary>
		/// <param name="parameters"><see cref="UnsetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void UnsetConfigValue(UnsetConfigValueParameters parameters)
		{
			GitConfigHelper.UnsetConfigValue(_executor, parameters);
		}

		/// <summary>Rename configuration section.</summary>
		/// <param name="parameters"><see cref="RenameConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void RenameConfigSection(RenameConfigSectionParameters parameters)
		{
			GitConfigHelper.RenameConfigSection(_executor, parameters);
		}

		/// <summary>Delete configuration section.</summary>
		/// <param name="parameters"><see cref="DeleteConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void DeleteConfigSection(DeleteConfigSectionParameters parameters)
		{
			GitConfigHelper.DeleteConfigSection(_executor, parameters);
		}
	}
}
