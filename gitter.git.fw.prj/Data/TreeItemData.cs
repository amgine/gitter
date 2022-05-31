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

namespace gitter.Git;

using System;

using gitter.Framework;

public abstract class TreeItemData : INamedObject
{
	private string _shortName;

	protected TreeItemData(string name, FileStatus fileStatus, StagedStatus stagedStatus)
	{
		Name         = name;
		FileStatus   = fileStatus;
		StagedStatus = stagedStatus;
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

	public FileStatus FileStatus { get; set; }

	public StagedStatus StagedStatus { get; set; }

	public string Name { get; }

	public override string ToString() => Name;
}
