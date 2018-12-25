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

	/// <summary>Branch description.</summary>
	public sealed class BranchData : INamedObject
	{
		#region .ctor

		public BranchData(string name, Hash sha1, bool isFake, bool isRemote, bool isCurrent)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, nameof(name));

			Name = name;
			SHA1 = sha1;
			IsFake = isFake;
			IsRemote = isRemote;
			IsCurrent = isCurrent;
		}

		public BranchData(string name, Hash sha1, bool remote, bool current)
			: this(name, sha1, false, remote, current)
		{
		}

		#endregion

		#region Properties

		/// <summary>Branche's name (short format, excluding refs/heads/ or /refs/%remote%/).</summary>
		public string Name { get; }

		/// <summary>SHA1 of commit, which is pointed by branch.</summary>
		public Hash SHA1 { get; }

		/// <summary>It's not actually a branch, just a representation of detached HEAD.</summary>
		public bool IsFake { get; }

		/// <summary>It is a remote tracking branch.</summary>
		public bool IsRemote { get; }

		/// <summary>This branch is current HEAD.</summary>
		public bool IsCurrent { get; }

		#endregion

		public override string ToString() => Name;
	}
}
