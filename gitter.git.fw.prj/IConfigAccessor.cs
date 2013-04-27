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

	/// <summary>Object which can perform various operations on git configuration.</summary>
	public interface IConfigAccessor
	{
		/// <summary>Query config parameter.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameterParameters"/>.</param>
		/// <returns><see cref="ConfigParameterData"/> for requested parameter or null if parameter does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		ConfigParameterData QueryConfigParameter(QueryConfigParameterParameters parameters);

		/// <summary>Query configuration parameter list.</summary>
		/// <param name="parameters"><see cref="QueryConfigParameters"/>.</param>
		/// <returns>List of requested parameters.</returns>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		IList<ConfigParameterData> QueryConfig(QueryConfigParameters parameters);

		/// <summary>Add config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void AddConfigValue(AddConfigValueParameters parameters);

		/// <summary>Set config value.</summary>
		/// <param name="parameters"><see cref="SetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void SetConfigValue(SetConfigValueParameters parameters);

		/// <summary>Unset config parameter value.</summary>
		/// <param name="parameters"><see cref="UnsetConfigValueParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void UnsetConfigValue(UnsetConfigValueParameters parameters);

		/// <summary>Rename configuration section.</summary>
		/// <param name="parameters"><see cref="RenameConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void RenameConfigSection(RenameConfigSectionParameters parameters);

		/// <summary>Delete configuration section.</summary>
		/// <param name="parameters"><see cref="DeleteConfigSectionParameters"/>.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		void DeleteConfigSection(DeleteConfigSectionParameters parameters);
	}
}
