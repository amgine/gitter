#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.AccessLayer
{
	using System;
	using System.Collections.Generic;

	using gitter.Git.AccessLayer.CLI;

	internal sealed partial class RepositoryCLI : IConfigAccessor
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
