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

namespace gitter.Git.AccessLayer.CLI
{
	using Resources = gitter.Git.AccessLayer.CLI.Properties.Resources;

	/// <summary>Provides accessor which works through MSysGit command line interface.</summary>
	public sealed class MSysGitAccessorProvider : IGitAccessorProvider
	{
		#region Properties

		/// <summary>Returns string used to identify git accessor.</summary>
		public string Name
		{
			get { return "MSysGit"; }
		}

		/// <summary>Returns string to represent accessor in GUI.</summary>
		public string DisplayName
		{
			get { return Resources.StrProviderName; }
		}

		#endregion

		#region Methods

		/// <summary>Creates git accessor.</summary>
		/// <returns>Created git accessor.</returns>
		public IGitAccessor CreateAccessor()
		{
			return new GitCLI(this);
		}

		#endregion
	}
}
