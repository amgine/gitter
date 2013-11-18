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
	using System.Collections.Generic;

	/// <summary>Object which can perform various operations on git configuration.</summary>
	public interface IConfigAccessor
	{
		/// <summary>Query configuration parameter list.</summary>
		IGitFunction<QueryConfigParameters, IList<ConfigParameterData>> QueryConfig { get; }

		/// <summary>Query config parameter.</summary>
		IGitFunction<QueryConfigParameterParameters, ConfigParameterData> QueryConfigParameter { get; }

		/// <summary>Add config value.</summary>
		IGitAction<AddConfigValueParameters> AddConfigValue { get; }

		/// <summary>Set config value.</summary>
		IGitAction<SetConfigValueParameters> SetConfigValue { get; }

		/// <summary>Unset config parameter value.</summary>
		IGitAction<UnsetConfigValueParameters> UnsetConfigValue { get; }

		/// <summary>Rename configuration section.</summary>
		IGitAction<RenameConfigSectionParameters> RenameConfigSection { get; }

		/// <summary>Delete configuration section.</summary>
		IGitAction<DeleteConfigSectionParameters> DeleteConfigSection { get; }
	}
}
