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
	public enum ReferenceChangeType
	{
		Added,
		Removed,
		Moved,
	}

	public sealed class ReferenceChange
	{
		#region Data

		private readonly ReferenceType _referenceType;
		private readonly string _fullName;
		private readonly string _name;
		private readonly string _oldHash;
		private readonly string _newHash;
		private readonly ReferenceChangeType _changeType;

		#endregion

		public ReferenceChange(
			ReferenceType referenceType,
			string fullName, string name,
			string oldHash, string newHash,
			ReferenceChangeType changeType)
		{
			_referenceType = referenceType;
			_fullName = fullName;
			_name = name;
			_oldHash = oldHash;
			_newHash = newHash;
			_changeType = changeType;
		}

		public ReferenceType ReferenceType
		{
			get { return _referenceType; }
		}

		public string FullName
		{
			get { return _fullName;}
		}

		public string Name
		{
			get { return _name;}
		}

		public string OldHash
		{
			get { return _oldHash; }
		}

		public string NewHash
		{
			get { return _newHash; }
		}

		public ReferenceChangeType ChangeType
		{
			get { return _changeType; }
		}
	}
}
