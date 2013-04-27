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

	public abstract class TreeItemData : INamedObject
	{
		private readonly string _name;
		private string _shortName;
		private FileStatus _fileStatus;
		private StagedStatus _stagedStatus;

		protected TreeItemData(string name, FileStatus fileStatus, StagedStatus stagedStatus)
		{
			_name = name;
			_fileStatus = fileStatus;
			_stagedStatus = stagedStatus;
		}

		public string ShortName
		{
			get
			{
				if(!string.IsNullOrEmpty(_shortName))
					return _shortName;
				var pos = Name.LastIndexOf('/');
				if(pos == -1)
					_shortName = Name;
				else
					_shortName = Name.Substring(pos + 1);
				return _shortName;
			}
			set { _shortName = value; }
		}

		public FileStatus FileStatus
		{
			get { return _fileStatus; }
			set { _fileStatus = value; }
		}

		public StagedStatus StagedStatus
		{
			get { return _stagedStatus; }
			set { _stagedStatus = value; }
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return _name;
		}
	}
}
