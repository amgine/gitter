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

	public sealed class RemoteData : INamedObject
	{
		#region Data

		private readonly string _name;
		private readonly string _fetchUrl;
		private readonly string _pushUrl;

		#endregion

		#region .ctor

		public RemoteData(string name, string fetchUrl, string pushUrl)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");
			Verify.Argument.IsNotNull(fetchUrl, "fetchUrl");
			Verify.Argument.IsNotNull(pushUrl, "pushUrl");

			_name = name;
			_fetchUrl = fetchUrl;
			_pushUrl = pushUrl;
		}

		#endregion

		#region Properties

		public string Name
		{
			get { return _name; }
		}

		public string FetchUrl
		{
			get { return _fetchUrl; }
		}

		public string PushUrl
		{
			get { return _pushUrl; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}
