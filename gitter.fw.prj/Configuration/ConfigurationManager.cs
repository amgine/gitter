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

namespace gitter.Framework.Configuration
{
	using System;

	public sealed class ConfigurationManager : INamedObject
	{
		private Section _rootSection;

		public ConfigurationManager(IDataAdapter dataAdapter)
		{
			Load(dataAdapter);
		}

		public ConfigurationManager(string name)
		{
			_rootSection = new Section(name);
		}

		public string Name => _rootSection.Name;

		public Section RootSection => _rootSection;

		public void Save(IDataAdapter dataAdapter)
		{
			Verify.Argument.IsNotNull(dataAdapter, nameof(dataAdapter));

			dataAdapter.Store(_rootSection);
		}

		public void Load(IDataAdapter dataAdapter)
		{
			Verify.Argument.IsNotNull(dataAdapter, nameof(dataAdapter));

			_rootSection = dataAdapter.Load();
		}

		public override string ToString() => Name;
	}
}
