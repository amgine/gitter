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

namespace gitter.TeamCity;

using System.Text;

public sealed class BuildTypeLocator : ObjectLocator
{
	public BuildTypeLocator()
	{
	}

	public string? Id { get; set; }

	public string? Name { get; set; }

	public override void ToString(StringBuilder sb)
	{
		if(!string.IsNullOrWhiteSpace(Id))
		{
			sb.Append("id:").Append(Id);
		}
		if(!string.IsNullOrWhiteSpace(Name))
		{
			sb.Append("name:").Append(Id);
		}
	}

	public override string ToString()
	{
		if(!string.IsNullOrWhiteSpace(Id))
		{
			return "id:" + Id;
		}
		if(!string.IsNullOrWhiteSpace(Name))
		{
			return "name:" + Name;
		}
		return string.Empty;
	}
}
