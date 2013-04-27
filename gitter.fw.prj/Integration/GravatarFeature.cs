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

namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using gitter.Framework.Services;

	using Resources = gitter.Framework.Properties.Resources;

	/// <summary>Gravatar integration feature.</summary>
	public sealed class GravatarFeature : IntegrationFeature
	{
		private DefaultGravatarType _defaultType;
		private const string _name = "Gravatar";

		internal GravatarFeature()
			: base(_name, Resources.StrsGravatarDisplayText, CachedResources.Bitmaps["ImgGravatar"], false)
		{
			IsEnabled = true;
		}

		public DefaultGravatarType DefaultGravatarType
		{
			get { return _defaultType; }
			set { _defaultType = value; }
		}
	}
}
