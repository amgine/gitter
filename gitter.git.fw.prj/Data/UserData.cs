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

	public sealed class UserData : INamedObject
	{
		#region Data

		private readonly string _name;
		private string _email;
		private int _commits;

		#endregion

		#region .ctor

		/// <summary>Create <see cref="UserData"/>.</summary>
		/// <param name="name">User name.</param>
		/// <param name="email">User email.</param>
		/// <param name="commits">Commit count.</param>
		public UserData(string name, string email, int commits)
		{
			_name = name;
			_email = email;
			_commits = commits;
		}

		#endregion

		#region Properties

		/// <summary>User id.</summary>
		public string Name
		{
			get { return _name + "\n" + _email; }
		}

		/// <summary>User name.</summary>
		public string UserName
		{
			get { return _name; }
		}

		/// <summary>User email.</summary>
		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}

		/// <summary>Commit count.</summary>
		public int Commits
		{
			get { return _commits; }
			set { _commits = value; }
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} <{1}>", _name, _email);
		}
	}
}
