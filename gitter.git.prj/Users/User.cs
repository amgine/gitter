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

namespace gitter.Git
{
	using System;

	using gitter.Framework;
	using gitter.Framework.Services;

	/// <summary>Committer/Author.</summary>
	public sealed class User : GitNamedObjectWithLifetime
	{
		private int _commits;

		/// <summary><see cref="Commits"/> has changed.</summary>
		public event EventHandler CommitsChanged;

		/// <summary>Create <see cref="User"/>.</summary>
		/// <param name="repository">Related repository.</param>
		/// <param name="name">User name.</param>
		/// <param name="email">User email.</param>
		/// <param name="commits">Commit count.</param>
		internal User(Repository repository, string name, string email, int commits)
			: base(repository, name + "\n" + email)
		{
			Name = name;
			Email = email;
			_commits = commits;
			Avatar = new Gravatar(email);
		}

		/// <summary>User name.</summary>
		public new string Name { get; }

		/// <summary>User email.</summary>
		public string Email { get; }

		/// <summary>User commit count.</summary>
		public int Commits
		{
			get => _commits;
			internal set
			{
				if(_commits != value)
				{
					_commits = value;
					CommitsChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>Avatar.</summary>
		public IAvatar Avatar { get; }
	}
}
