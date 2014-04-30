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

	public sealed class RemoteReferenceData
	{
		#region Data

		private readonly string _name;
		private readonly Hash _hash;
		private TagType _tagType;

		#endregion

		#region .ctor

		public RemoteReferenceData(string name, Hash hash)
		{
			Verify.Argument.IsNeitherNullNorWhitespace(name, "name");

			_name = name;
			_hash = hash;
		}

		#endregion

		#region Properties

		public TagType TagType
		{
			get { return _tagType; }
			set { _tagType = value; }
		}

		public ReferenceType ReferenceType
		{
			get
			{
				if(_name.StartsWith(GitConstants.LocalBranchPrefix))
					return ReferenceType.LocalBranch;
				if(_name.StartsWith(GitConstants.TagPrefix))
					return ReferenceType.Tag;
				return ReferenceType.Reference;
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public Hash Hash
		{
			get { return _hash; }
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}
