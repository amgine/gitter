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

	using gitter.Framework;

	public sealed class ConfigParameterData : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _value;
		private readonly ConfigFile _configFile;
		private readonly string _fileName;

		#endregion

		#region .ctor

		public ConfigParameterData(string name, string value, ConfigFile configFile, string fileName)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(value, "value");

			_name = name;
			_value = value;
			_configFile = configFile;
			_fileName = fileName;
		}

		public ConfigParameterData(string name, string value, ConfigFile configFile)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(value, "value");
			Verify.Argument.AreNotEqual(ConfigFile.Other, configFile, "configFile", string.Empty);

			_name = name;
			_value = value;
			_configFile = configFile;
		}

		public ConfigParameterData(string name, string value, string fileName)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(value, "value");
			Verify.Argument.IsNeitherNullNorWhitespace(fileName, "fileName");

			_name = name;
			_value = value;
			_configFile = ConfigFile.Other;
			_fileName = fileName;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public string Value
		{
			get { return _value; }
		}

		public ConfigFile ConfigFile
		{
			get { return _configFile; }
		}

		public string SpecifiedFile
		{
			get { return _fileName; }
		}

		#endregion
	}
}
