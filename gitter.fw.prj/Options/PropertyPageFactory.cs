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

namespace gitter.Framework.Options
{
	using System;
	using System.Drawing;

	public sealed class PropertyPageFactory
	{
		#region Static Data

		public static readonly Guid RootGroupGuid		= Guid.Empty;
		public static readonly Guid AppearanceGroupGuid = new Guid("F1F07910-1105-4928-9B7C-F62657601747");

		#endregion

		#region Data

		private readonly Guid _guid;
		private readonly Guid _groupGuid;
		private readonly string _name;
		private Func<IWorkingEnvironment, PropertyPage> _getPropertyPage;
		private Bitmap _icon;

		#endregion

		#region .ctor

		public PropertyPageFactory(Guid guid, string name, Bitmap icon, Guid groupGuid, Func<IWorkingEnvironment, PropertyPage> getPropertyPage)
		{
			_guid = guid;
			_name = name;
			_groupGuid = groupGuid;
			_icon = icon;
			_getPropertyPage = getPropertyPage;
		}

		#endregion

		#region Properties

		public Guid Guid
		{
			get { return _guid; }
		}

		public Guid GroupGuid
		{
			get { return _groupGuid; }
		}

		public string Name
		{
			get { return _name; }
		}

		public Bitmap Icon
		{
			get { return _icon; }
		}

		#endregion

		#region Methods

		public PropertyPage CreatePropertyPage(IWorkingEnvironment environment)
		{
			Verify.Argument.IsNotNull(environment, "environment");

			if(_getPropertyPage != null)
			{
				return _getPropertyPage(environment);
			}
			else
			{
				return null;
			}
		}

		#endregion
	}
}
