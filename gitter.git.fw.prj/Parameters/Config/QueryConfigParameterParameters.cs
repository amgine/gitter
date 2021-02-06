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

	/// <summary>Parameters for <see cref="IConfigAccessor.QueryConfigParameter"/> operation.</summary>
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
