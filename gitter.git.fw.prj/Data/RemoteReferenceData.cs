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

namespace gitter.Git.AccessLayer;

public sealed class RemoteReferenceData
{
	public RemoteReferenceData(string name, Sha1Hash hash)
	{
		Verify.Argument.IsNeitherNullNorWhitespace(name);

		Name = name;
		Hash = hash;
	}

	public TagType TagType { get; set; }

	public ReferenceType ReferenceType
	{
		get
		{
			if(Name.StartsWith(GitConstants.LocalBranchPrefix))
				return ReferenceType.LocalBranch;
			if(Name.StartsWith(GitConstants.TagPrefix))
				return ReferenceType.Tag;
			return ReferenceType.Reference;
		}
	}

	public string Name { get; }

	public Sha1Hash Hash { get; }

	public override string ToString() => Name;
}
